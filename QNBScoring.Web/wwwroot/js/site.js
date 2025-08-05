// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Activer les tooltips
document.addEventListener('DOMContentLoaded', function () {
    // Tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    // Tri du tableau
    document.querySelectorAll('.sortable').forEach(header => {
        header.addEventListener('click', () => {
            const table = header.closest('table');
            const tbody = table.querySelector('tbody');
            const rows = Array.from(tbody.querySelectorAll('tr'));
            const index = Array.from(header.parentNode.children).indexOf(header);
            const direction = header.classList.contains('asc') ? -1 : 1;

            // Toggle direction
            header.classList.toggle('asc', !header.classList.contains('asc'));

            // Reset other headers
            header.parentNode.querySelectorAll('.sortable').forEach(h => {
                if (h !== header) h.classList.remove('asc', 'desc');
            });

            // Sort rows
            rows.sort((a, b) => {
                const aValue = a.children[index].textContent.trim();
                const bValue = b.children[index].textContent.trim();

                // Try to compare as numbers
                if (!isNaN(aValue) && !isNaN(bValue)) {
                    return (parseFloat(aValue) - parseFloat(bValue)) * direction;
                }

                // Compare as dates
                if (Date.parse(aValue) && Date.parse(bValue)) {
                    return (Date.parse(aValue) - Date.parse(bValue)) * direction;
                }

                // Default string comparison
                return aValue.localeCompare(bValue) * direction;
            });

            // Re-add rows
            rows.forEach(row => tbody.appendChild(row));
        });
    });

    // Recherche dans le tableau
    const searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('input', () => {
            const searchTerm = searchInput.value.toLowerCase();
            const rows = document.querySelectorAll('#demandesTable tbody tr');

            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(searchTerm) ? '' : 'none';
            });
        });
    }
});