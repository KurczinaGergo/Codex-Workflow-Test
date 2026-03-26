# Domain Model Reference

<!-- FILL IN: Authoritative reference for entities, enumerations, and invariants.
     Read by the implementation agent when touching domain-related code.
     Rule: if this file exceeds 150 lines, extract entity groups to docs/domain-<context>.md
     and replace with a one-line pointer per group.
     Update after every review session that changes an entity.
     Check docs/progress/design-state.md for deviations not yet incorporated here. -->

---

## Entity Overview

<!-- FILL IN: Tree or diagram showing entity relationships.

     Example:
     Order
       ├── OrderLine[]
       └── Customer
-->

---

## Entities

<!-- FILL IN: One table per entity. Fields, types, and notes on non-obvious constraints.

     Example:
     ### Order
     | Field       | Type        | Notes                            |
     |-------------|-------------|----------------------------------|
     | Id          | Guid        |                                  |
     | Status      | OrderStatus | see state machine below          |
     | TotalAmount | decimal     | locked at placement, never recalc|
-->

---

## Enumerations

<!-- FILL IN: Enum values and state machine transitions where applicable.

     Example:
     ### OrderStatus
     Pending → Confirmed → Shipped → Delivered
                    ↓
                Cancelled  (only from Pending or Confirmed)
-->

---

## Key Invariants

<!-- FILL IN: Business rules enforced at domain level. Never bypass, even in tests.

     Example:
     - Order.TotalAmount is set at placement and never recalculated
     - A Confirmed order cannot have zero OrderLines
-->
