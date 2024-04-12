document.querySelectorAll('.deleteButton').forEach(function(button) {
    button.addEventListener('click', function(event) {
        var confirmed = confirm('Are you sure you want to delete this record?');
        if (!confirmed) {
            event.preventDefault(); // Stop the form from submitting
        }
    });
});
