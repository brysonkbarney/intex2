document.addEventListener('DOMContentLoaded', (event) => {
    const pageSizeSelect = document.querySelector('select[name="pageSize"]');
    if (pageSizeSelect) {
        pageSizeSelect.addEventListener('change', function() {
            this.form.submit();
        });
    }
});