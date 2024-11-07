$(document).ready(function () {
    // Caching the DOM elements
    const $hoursWorked = $('input[name="hoursWorked"]');
    const $hourlyRate = $('input[name="hourlyRate"]');
    const $form = $('form');
    const $fileInput = $('input[type="file"]');
    const $totalAmountDisplay = $('.total-amount-display');

    // Function to calculate and display total amount of a lecturer's claim
    function calculateTotal() {
        const hours = parseFloat($hoursWorked.val()) || 0;
        const rate = parseFloat($hourlyRate.val()) || 0;
        const total = (hours * rate).toFixed(2);
        if (hours && rate) {
            $totalAmountDisplay.html(`<strong>Total Amount: R${total}</strong>`);
        } else {
            $totalAmountDisplay.html('<span style="color: #6c757d;">Enter hours and rate to see total</span>');
        }
    }

    // Validation functions
    function validateHours(hours) {
        return hours > 0; // Only checking that hours is positive
    }

    function validateRate(rate) {
        return rate > 0; // Only checking that rate is positive
    }

    function validateFileSize() {
        // Function to validate the size of a file
        const fileInput = $fileInput[0];
        if (fileInput.files.length > 0) {
            const fileSize = fileInput.files[0].size / 1024 / 1024; // Convert to MB
            return fileSize <= 5; // Max 5MB for file size
        }
        return true;
    }

    // Event listeners for input fields
    $hoursWorked.on('input', calculateTotal);
    $hourlyRate.on('input', calculateTotal);

    // File input validation
    $fileInput.on('change', function () {
        const $existingError = $(this).next('.invalid-feedback');
        if (!validateFileSize()) {
            $(this).addClass('is-invalid');
            if ($existingError.length === 0) {
                $(this).after('<div class="invalid-feedback">File size must not exceed 5MB</div>');
            }
        } else {
            $(this).removeClass('is-invalid');
            $('.invalid-feedback').remove();
        }
    });

    // Form submission handling with validation
    $form.on('submit', function (e) {
        const hours = parseFloat($hoursWorked.val());
        const rate = parseFloat($hourlyRate.val());

        // Clear existing error messages
        $('.invalid-feedback').remove();
        $('.is-invalid').removeClass('is-invalid');

        let isValid = true;

        // Validate the lecturer's hours worked
        if (!validateHours(hours)) {
            $hoursWorked.addClass('is-invalid');
            $hoursWorked.after('<div class="invalid-feedback">Please enter a positive number of hours</div>');
            isValid = false;
        }

        // Validate the lecturer's hourly rate
        if (!validateRate(rate)) {
            $hourlyRate.addClass('is-invalid');
            $hourlyRate.after('<div class="invalid-feedback">Please enter a positive rate</div>');
            isValid = false;
        }

        // Validate file size
        if (!validateFileSize()) {
            $fileInput.addClass('is-invalid');
            $fileInput.after('<div class="invalid-feedback">File size must not exceed 5MB</div>');
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault(); // Prevent form submission if validation fails
        }
    });
});
