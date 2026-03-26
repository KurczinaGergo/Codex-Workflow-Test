# Review File — order-processing (example)

<!-- This is a filled example showing what a well-written review file looks like.
     It uses a fictional e-commerce order processing feature.
     Use it as a reference when reading your own agent-generated review files,
     and as a calibration tool when prompting your implementation agent.

     A good review file:
     - Fits in one reasoning partner context window (aim for under 800 lines)
     - Contains no boilerplate or configuration code
     - Surfaces every non-trivial decision with its reasoning
     - Is honest about deviations — never buries them
     - Asks only real questions in section 6, not rhetorical ones -->

---

## 1. Summary

Implements the order processing flow as specified in `docs/specs/order-processing.md`.
Covers the domain entities (`Order`, `OrderLine`), the `PlaceOrderCommand` with its handler
and validator, EF Core persistence configuration, and a thin API endpoint.
The project is currently in the initial domain build-out phase — no sprint features have been
implemented yet. All tests pass.

---

## 2. Changed files

| File | Layer | Change |
|------|-------|--------|
| `src/Domain/Entities/Order.cs` | Domain | new |
| `src/Domain/Entities/OrderLine.cs` | Domain | new |
| `src/Domain/Events/OrderPlacedEvent.cs` | Domain | new |
| `src/Domain/Interfaces/IOrderRepository.cs` | Domain | new |
| `src/Application/Commands/PlaceOrderCommand.cs` | Application | new |
| `src/Application/Commands/PlaceOrderCommandHandler.cs` | Application | new |
| `src/Application/Validators/PlaceOrderCommandValidator.cs` | Application | new |
| `src/Infrastructure/Persistence/OrderConfiguration.cs` | Infrastructure | new |
| `src/Infrastructure/Persistence/EfCoreOrderRepository.cs` | Infrastructure | new |
| `src/API/Controllers/OrderController.cs` | API | new |
| `tests/Domain.Tests/OrderTests.cs` | Tests | new |
| `tests/Application.Tests/PlaceOrderCommandHandlerTests.cs` | Tests | new |

---

## 3. Key decisions made during implementation

**Decision 1: `OrderLine` is an owned entity, not a separate aggregate**
- What: `OrderLine` is configured as EF Core owned type inside `Order`, sharing the same table via a join table, not a standalone DbSet.
- Why: `OrderLine` has no identity outside of its `Order`. Making it an owned type enforces this at the infrastructure level and removes the need for a separate `IOrderLineRepository`.
- Alternative rejected: a separate `OrderLine` entity with its own repository — this would allow `OrderLine` to be queried and modified independently, which violates the aggregate boundary defined in the domain model.

**Decision 2: `Order.TotalAmount` is calculated on placement, not derived on read**
- What: `TotalAmount` is a stored field on `Order`, calculated and set in `Order.Place()` at the moment the order is created. It is not a computed property recalculated from `OrderLines` on every read.
- Why: the spec states "total amount must be locked at placement time." This makes the stored value the record of what the customer agreed to, regardless of future price changes.
- Alternative rejected: `decimal TotalAmount => OrderLines.Sum(l => l.UnitPrice * l.Quantity)` — this would silently change if `UnitPrice` were mutable, which is a correctness risk.

**Decision 3: `PlaceOrderCommand` validates minimum one `OrderLine` in the validator, not the domain entity**
- What: the rule "an order must have at least one line" is enforced by `PlaceOrderCommandValidator`, not inside `Order.Place()`.
- Why: the spec did not specify where this invariant should live. Placing it in the validator keeps the domain entity constructor simpler and provides a better error message to the API caller. However, this is a deviation from the general principle that invariants belong in the domain — see section 5.
- Alternative rejected: throwing a `DomainException` inside `Order.Place()` — this is more correct from a pure domain perspective but loses the FluentValidation error shape.

---

## 4. Critical code excerpts

### Order aggregate root

```csharp
public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime PlacedAt { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    private Order() { } // EF Core

    public static Order Place(Guid customerId, IEnumerable<OrderLine> lines)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            PlacedAt = DateTime.UtcNow
        };

        foreach (var line in lines)
            order._lines.Add(line);

        order.TotalAmount = order._lines.Sum(l => l.UnitPrice * l.Quantity);

        order.AddDomainEvent(new OrderPlacedEvent(order.Id, order.TotalAmount));

        return order;
    }
}
```

### IOrderRepository

```csharp
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
}
```

---

## 5. Spec deviations

**Invariant location for minimum order lines.**
The spec states: _"An order must contain at least one line item."_
The spec does not specify whether this is a domain invariant or an application-level validation rule.
This implementation enforces it in `PlaceOrderCommandValidator`, not in `Order.Place()`.

Consequence: it is possible to construct an `Order` in tests with zero lines without triggering a domain exception. This is a deliberate trade-off for better API error messages, but it weakens the domain model slightly.

Recommendation for reasoning partner: decide whether this should be moved into the domain entity and recorded in an ADR.

---

## 6. Open questions for the reasoning partner

**Q1: Should the minimum-lines invariant move to the domain entity?**
See section 5. If yes, `Order.Place()` should throw `DomainException` when passed an empty lines collection, and the validator check becomes redundant. This is a domain modelling decision — not something the implementation agent should resolve unilaterally.

**Q2: Should `Order.TotalAmount` be recalculated if an order is modified after placement?**
The current model assumes orders are immutable after placement. If a future feature allows adding lines to a pending order, `TotalAmount` will need a recalculation method. No such feature is in the backlog currently, but the architecture should account for it if it is likely.

---

## 7. Test coverage

**Added:**
- `OrderTests.Place_WithValidLines_SetsCorrectTotalAmount` — verifies TotalAmount calculation
- `OrderTests.Place_RaisesOrderPlacedEvent` — verifies domain event is raised with correct values
- `PlaceOrderCommandHandlerTests.Handle_ValidCommand_PersistsOrderAndReturnsId` — happy path
- `PlaceOrderCommandHandlerTests.Handle_EmptyLines_FailsValidation` — validator boundary

**Not covered:**
- EF Core persistence configuration — requires integration test with a real database. Out of scope for this feature; a separate integration test suite is planned.
- `Order` with a very large number of lines — no performance test exists. Not a current concern for the expected data volume.

---

## 8. Proposed documentation updates

- `docs/domain-model.md` → Add `Order` and `OrderLine` entity tables. Add `OrderStatus` enum with state machine (`Pending → Confirmed → Shipped → Delivered`, `Cancelled` from `Pending` or `Confirmed` only). Add invariant: _"TotalAmount is set at placement and never recalculated."_
- `docs/architecture.md` → Add note under EF Core rules: _"Aggregate-internal entities (e.g. OrderLine) are configured as owned types, not standalone DbSets."_
- New ADR required: **yes** — _"Invariant enforcement location: domain entity vs. application validator."_ Draft: The project must decide whether invariants that affect API error message quality (e.g. minimum order lines) are enforced in the domain entity (throws DomainException, caught and mapped to 422) or in the FluentValidation layer (returns structured validation errors). A consistent rule prevents case-by-case decisions during implementation.
