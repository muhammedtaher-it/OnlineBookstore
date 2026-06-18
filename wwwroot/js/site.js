// Online Bookstore - JavaScript

// Auto-hide alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    var alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            var closeButton = alert.querySelector('.btn-close');
            if (closeButton) {
                closeButton.click();
            }
        }, 5000);
    });
});

// Confirm delete actions
document.addEventListener('DOMContentLoaded', function () {
    var deleteForms = document.querySelectorAll('form[onsubmit*="confirm"]');
    deleteForms.forEach(function (form) {
        form.addEventListener('submit', function (e) {
            if (!confirm('Are you sure you want to proceed with this action?')) {
                e.preventDefault();
            }
        });
    });
});

// Cart quantity update
document.addEventListener('DOMContentLoaded', function () {
    var quantityInputs = document.querySelectorAll('input[name="quantity"]');
    quantityInputs.forEach(function (input) {
        input.addEventListener('change', function () {
            if (this.value < 1) this.value = 1;
            if (this.value > 99) this.value = 99;
        });
    });
});

// Star rating hover effect
document.addEventListener('DOMContentLoaded', function () {
    var ratingLabels = document.querySelectorAll('.rating-input label');
    ratingLabels.forEach(function (label) {
        label.addEventListener('mouseenter', function () {
            this.style.transform = 'scale(1.1)';
        });
        label.addEventListener('mouseleave', function () {
            this.style.transform = 'scale(1)';
        });
    });
});
