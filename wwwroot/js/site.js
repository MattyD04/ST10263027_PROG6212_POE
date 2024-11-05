// Wait for the document to be ready
$(document).ready(function () {
    // Cache DOM elements
    const $hoursWorked = $('input[name="hoursWorked"]');
    const $hourlyRate = $('input[name="hourlyRate"]');
    const $form = $('form');
    const $fileInput = $('input[type="file"]');

    // Add a new element to display total amount
    const $totalAmountDisplay = $('<div class="total-amount-display" style="margin: 15px 0; padding: 10px; background-color: #e9ecef; border-radius: 4px;"></div>');
    $hourlyRate.after($totalAmountDisplay);

    // Function to calculate and display total amount
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

    // Input validation functions
    function validateHours(hours) {
        return hours > 0 && hours <= 160; // Maximum 160 hours per month
    }

    function validateRate(rate) {
        return rate > 0 && rate <= 1000; // Maximum rate of R1000 per hour
    }

    function validateFileSize() {
        const fileInput = $fileInput[0];
        if (fileInput.files.length > 0) {
            const fileSize = fileInput.files[0].size / 1024 / 1024; // Convert to MB
            return fileSize <= 5; // Max 5MB
        }
        return true;
    }

    // Add input event listeners for real-time calculation
    $hoursWorked.on('input', function () {
        const hours = parseFloat($(this).val());
        if (!validateHours(hours)) {
            $(this).addClass('is-invalid');
            if (hours > 160) {
                $(this).after('<div class="invalid-feedback">Hours cannot exceed 160 per month</div>');
            } else {
                $(this).after('<div class="invalid-feedback">Please enter valid hours worked</div>');
            }
        } else {
            $(this).removeClass('is-invalid');
            $('.invalid-feedback').remove();
        }
        calculateTotal();
    });

    $hourlyRate.on('input', function () {
        const rate = parseFloat($(this).val());
        if (!validateRate(rate)) {
            $(this).addClass('is-invalid');
            if (rate > 1000) {
                $(this).after('<div class="invalid-feedback">Hourly rate cannot exceed R1000</div>');
            } else {
                $(this).after('<div class="invalid-feedback">Please enter a valid hourly rate</div>');
            }
        } else {
            $(this).removeClass('is-invalid');
            $('.invalid-feedback').remove();
        }
        calculateTotal();
    });

    // File input validation
    $fileInput.on('change', function () {
        if (!validateFileSize()) {
            $(this).addClass('is-invalid');
            $(this).after('<div class="invalid-feedback">File size must not exceed 5MB</div>');
        } else {
            $(this).removeClass('is-invalid');
            $('.invalid-feedback').remove();
        }
    });

    // Form submission handling
    $form.on('submit', function (e) {
        const hours = parseFloat($hoursWorked.val());
        const rate = parseFloat($hourlyRate.val());

        // Clear any existing error messages
        $('.invalid-feedback').remove();
        $('.is-invalid').removeClass('is-invalid');

        let isValid = true;

        // Validate hours
        if (!validateHours(hours)) {
            $hoursWorked.addClass('is-invalid');
            $hoursWorked.after('<div class="invalid-feedback">Please enter valid hours (1-160)</div>');
            isValid = false;
        }

        // Validate rate
        if (!validateRate(rate)) {
            $hourlyRate.addClass('is-invalid');
            $hourlyRate.after('<div class="invalid-feedback">Please enter valid rate (1-1000)</div>');
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