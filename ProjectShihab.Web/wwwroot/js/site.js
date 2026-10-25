// Sidebar toggle for small screens.
document.addEventListener("click", function (e) {
    var sidebar = document.getElementById("sidebar");
    if (!sidebar) return;
    if (e.target.closest("[data-sidebar-toggle]")) {
        sidebar.classList.toggle("open");
    } else if (e.target.classList.contains("sidebar-backdrop")) {
        sidebar.classList.remove("open");
    }
});

// Confirm dialogs for destructive forms: <form data-confirm="Are you sure?">
document.addEventListener("submit", function (e) {
    var msg = e.target.getAttribute("data-confirm");
    if (msg && !window.confirm(msg)) e.preventDefault();
});

// Live client-side table filter: <input data-table-filter="#tableId">
document.addEventListener("input", function (e) {
    var sel = e.target.getAttribute("data-table-filter");
    if (!sel) return;
    var q = e.target.value.toLowerCase();
    document.querySelectorAll(sel + " tbody tr").forEach(function (tr) {
        tr.style.display = tr.textContent.toLowerCase().indexOf(q) > -1 ? "" : "none";
    });
});
