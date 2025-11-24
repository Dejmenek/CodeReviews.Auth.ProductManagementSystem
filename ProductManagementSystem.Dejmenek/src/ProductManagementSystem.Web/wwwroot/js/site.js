document.addEventListener("DOMContentLoaded", function () {
    var sidebar = document.getElementById("sidebar");
    var collapseBtn = document.getElementById("sidebarCollapse");
    var toggleIcon = document.getElementById("sidebarToggleIcon");

    if (sidebar && collapseBtn && toggleIcon) {
        collapseBtn.addEventListener("click", function () {
            sidebar.classList.toggle("sidebar-collapsed");
            if (sidebar.classList.contains("sidebar-collapsed")) {
                toggleIcon.classList.remove("bi-x");
                toggleIcon.classList.add("bi-list");
            } else {
                toggleIcon.classList.remove("bi-list");
                toggleIcon.classList.add("bi-x");
            }
        });
        // Set initial icon state
        if (sidebar.classList.contains("sidebar-collapsed")) {
            toggleIcon.classList.remove("bi-x");
            toggleIcon.classList.add("bi-list");
        } else {
            toggleIcon.classList.remove("bi-list");
            toggleIcon.classList.add("bi-x");
        }
    }
});

function toggleAll(checked) {
    document.querySelectorAll('.row-checkbox').forEach(cb => cb.checked = checked);
}

function submitBulkDelete() {
    const form = document.getElementById('bulkDeleteForm');

    document.querySelectorAll('.row-checkbox:checked').forEach(cb => {
        const hidden = document.createElement('input');
        hidden.type = 'hidden';
        hidden.name = 'selectedIds';
        hidden.value = cb.value;
        form.appendChild(hidden);
    });

    const pageSizeSelect = document.querySelector('select[name="Filter.PageSize"]');
    if (pageSizeSelect) {
        const pageSizeInput = document.createElement('input');
        pageSizeInput.type = 'hidden';
        pageSizeInput.name = 'Filter.PageSize';
        pageSizeInput.value = pageSizeSelect.value;
        form.appendChild(pageSizeInput);
    }

    const searchInput = document.querySelector('input[name="Filter.Search"]');
    if (searchInput) {
        const searchHidden = document.createElement('input');
        searchHidden.type = 'hidden';
        searchHidden.name = 'Filter.Search';
        searchHidden.value = searchInput.value;
        form.appendChild(searchHidden);
    }

    form.submit();
}