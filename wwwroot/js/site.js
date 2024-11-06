$(document).ready(function () {
    // Caching the DOM elements
    const $hoursWorked = $('input[name="hoursWorked"]');
    const $hourlyRate = $('input[name="hourlyRate"]');
    const $form = $('form');
    const $fileInput = $('input[type="file"]');
    const $totalAmountDisplay = $('.total-amount-display');
    const $submitButton = $('button[type="submit"]'); // Submit button for validation

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

    // Validation Functions
    function validateHours(hours) {
        return hours > 0; // Checks that hours is positive
    }

    function validateRate(rate) {
        return rate > 0; // Checks that rate is positive
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

    // Input event listeners for calculation
    $hoursWorked.on('input', function () {
        calculateTotal(); // Calling the calculateTotal method 
    });

    $hourlyRate.on('input', function () {
        calculateTotal(); // Calling the calculateTotal method 
    });

    // Change event listener for file input to validate file size
    $fileInput.on('change', function () {
        const $existingError = $(this).next('.invalid-feedback');
        const isFileValid = validateFileSize();

        if (isFileValid) {
            $existingError.remove();
        } else {
            if ($existingError.length === 0) {
                $(this).after('<div class="invalid-feedback">File size should be less than 5 MB.</div>');
            }
        }
    });

    // Submit form validation
    $form.on('submit', function (e) {
        const hours = parseFloat($hoursWorked.val()) || 0;
        const rate = parseFloat($hourlyRate.val()) || 0;
        const fileIsValid = validateFileSize();

        // Prevent form submission if validation fails
        if (!validateHours(hours)) {
            e.preventDefault();
            alert('Please enter valid hours worked.');
            return;
        }

        if (!validateRate(rate)) {
            e.preventDefault();
            alert('Please enter a valid hourly rate.');
            return;
        }

        if (!fileIsValid) {
            e.preventDefault();
            alert('The file size must be less than 5 MB.');
            return;
        }

        // Proceed with form submission if all validations pass
        return true;
    });

    // Enable/Disable Submit button based on input values
    function toggleSubmitButton() {
        const hours = parseFloat($hoursWorked.val()) || 0;
        const rate = parseFloat($hourlyRate.val()) || 0;
        const fileIsValid = validateFileSize();

        if (hours > 0 && rate > 0 && fileIsValid) {
            $submitButton.prop('disabled', false);
        } else {
            $submitButton.prop('disabled', true);
        }
    }

    // Monitor inputs to enable/disable the submit button dynamically
    $hoursWorked.on('input', toggleSubmitButton);
    $hourlyRate.on('input', toggleSubmitButton);
    $fileInput.on('change', toggleSubmitButton);
});
