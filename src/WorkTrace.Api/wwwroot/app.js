const api = {
  async request(path, options = {}) {
    const response = await fetch(path, {
      headers: {
        "Content-Type": "application/json",
        ...(options.headers ?? {})
      },
      ...options
    });

    if (!response.ok) {
      const problem = await response.json().catch(() => null);
      throw new Error(problem?.detail ?? response.statusText);
    }

    if (response.status === 204) {
      return null;
    }

    return response.json();
  }
};

const state = {
  workItems: [],
  projects: [],
  timeline: [],
  activeWork: null
};

const elements = {
  workItems: document.getElementById("workItems"),
  projects: document.getElementById("projects"),
  timeline: document.getElementById("timeline"),
  activeStatus: document.getElementById("activeStatus"),
  activeMeta: document.getElementById("activeMeta"),
  refreshButton: document.getElementById("refreshButton"),
  workItemForm: document.getElementById("workItemForm"),
  projectForm: document.getElementById("projectForm")
};

async function loadDashboard() {
  const [workItems, projects, timeline, activeWork] = await Promise.all([
    api.request("/api/work-items"),
    api.request("/api/projects"),
    api.request("/api/timeline"),
    api.request("/api/active-work")
  ]);

  state.workItems = workItems;
  state.projects = projects;
  state.timeline = timeline;
  state.activeWork = activeWork;

  render();
}

function render() {
  renderActiveWork();
  renderWorkItems();
  renderProjects();
  renderTimeline();
}

function renderActiveWork() {
  const activeSession = state.activeWork?.activeSession;
  const activeWorkItem = state.activeWork?.activeWorkItem;

  if (!activeSession) {
    elements.activeStatus.textContent = "No active session";
    elements.activeMeta.textContent = "Start a work session to begin tracking time.";
    return;
  }

  elements.activeStatus.textContent = activeWorkItem?.title ?? "Active session";
  elements.activeMeta.textContent = `Started ${formatDate(activeSession.startedAt)} on ${activeWorkItem?.status ?? "unknown"}`;
}

function renderWorkItems() {
  elements.workItems.innerHTML = state.workItems.map(workItem => {
    const sessionLabel = workItem.sessionCount === 1 ? "1 session" : `${workItem.sessionCount} sessions`;
    const noteLabel = workItem.noteCount === 1 ? "1 note" : `${workItem.noteCount} notes`;
    return `
      <article class="card">
        <div class="card-head">
          <div>
            <strong>${escapeHtml(workItem.title)}</strong>
            <div class="muted">${escapeHtml(workItem.description ?? "No description yet")}</div>
          </div>
          <span class="chip">${workItem.status}</span>
        </div>
        <div class="chip-row">
          <span class="chip">${workItem.kind}</span>
          <span class="chip">${sessionLabel}</span>
          <span class="chip">${noteLabel}</span>
          ${workItem.isArchived ? '<span class="chip">Archived</span>' : ""}
        </div>
        <div class="card-actions">
          <button data-action="start-session" data-id="${workItem.id}">Start session</button>
          <button data-action="status" data-status="InProgress" data-id="${workItem.id}">Set In Progress</button>
          <button data-action="status" data-status="Done" data-id="${workItem.id}">Set Done</button>
          <button data-action="archive" data-id="${workItem.id}">Archive</button>
          <button data-action="note" data-id="${workItem.id}">Add note</button>
        </div>
      </article>
    `;
  }).join("");

  elements.workItems.querySelectorAll("button").forEach(button => {
    button.addEventListener("click", handleWorkItemAction);
  });
}

function renderProjects() {
  elements.projects.innerHTML = state.projects.map(project => `
    <article class="card">
      <strong>${escapeHtml(project.name)}</strong>
      <div class="muted">Updated ${formatDate(project.updatedAt)}</div>
    </article>
  `).join("");
}

function renderTimeline() {
  elements.timeline.innerHTML = state.timeline.map(entry => `
    <article class="card">
      <div class="card-head">
        <strong>${escapeHtml(entry.kind)}</strong>
        <span class="muted">${formatDate(entry.occurredAt)}</span>
      </div>
      <div>${escapeHtml(entry.message)}</div>
    </article>
  `).join("");
}

async function handleWorkItemAction(event) {
  const button = event.currentTarget;
  const id = button.dataset.id;
  const action = button.dataset.action;

  try {
    if (action === "start-session") {
      await api.request(`/api/work-items/${id}/sessions`, { method: "POST", body: "{}" });
    } else if (action === "status") {
      await api.request(`/api/work-items/${id}/status`, {
        method: "PATCH",
        body: JSON.stringify({ status: button.dataset.status })
      });
    } else if (action === "archive") {
      await api.request(`/api/work-items/${id}/archive`, { method: "POST", body: "{}" });
    } else if (action === "note") {
      const text = prompt("Note text");
      if (!text) {
        return;
      }

      await api.request(`/api/work-items/${id}/notes`, {
        method: "POST",
        body: JSON.stringify({ text, type: "Human" })
      });
    }

    await loadDashboard();
  } catch (error) {
    alert(error.message);
  }
}

function wireForms() {
  elements.refreshButton.addEventListener("click", loadDashboard);

  elements.workItemForm.addEventListener("submit", async event => {
    event.preventDefault();
    const form = new FormData(elements.workItemForm);
    await api.request("/api/work-items", {
      method: "POST",
      body: JSON.stringify({
        title: form.get("title"),
        kind: form.get("kind"),
        description: form.get("description")
      })
    });
    elements.workItemForm.reset();
    await loadDashboard();
  });

  elements.projectForm.addEventListener("submit", async event => {
    event.preventDefault();
    const form = new FormData(elements.projectForm);
    await api.request("/api/projects", {
      method: "POST",
      body: JSON.stringify({ name: form.get("name") })
    });
    elements.projectForm.reset();
    await loadDashboard();
  });
}

function escapeHtml(value) {
  return String(value)
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#39;");
}

function formatDate(value) {
  return new Date(value).toLocaleString(undefined, {
    dateStyle: "medium",
    timeStyle: "short"
  });
}

wireForms();
loadDashboard().catch(error => {
  elements.activeStatus.textContent = "Offline";
  elements.activeMeta.textContent = error.message;
});
