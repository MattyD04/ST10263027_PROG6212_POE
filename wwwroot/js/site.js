
$(document).ready(function () {
    // Cache DOM elements
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

    
    function validateHours(hours) {
        return hours > 0 && hours <= 200; // Maximum 200 hours per month for a lecturer to be able to work
    }

    function validateRate(rate) {
        return rate > 0 && rate <= 1000; // Maximum rate of R1000 per hour for a lecturer's work
    }

    function validateFileSize() {
        const fileInput = $fileInput[0];
        if (fileInput.files.length > 0) {
            const fileSize = fileInput.files[0].size / 1024 / 1024; // Convert to MB
            return fileSize <= 5; // Max 5MB
        }
        return true;
    }

   
    $hoursWorked.on('input', function () {
        calculateTotal(); //calling the calculateTotal method 
    });

    $hourlyRate.on('input', function () {
        calculateTotal(); //calling the calculateTotal method 
    });

    
    $fileInput.on('change', function () {
        const $existingError = $(this).next('.invalid-feedback'); 

        if (!validateFileSize()) {
            $(this).addClass('is-invalid');
            if ($existingError.length === 0) {
                $(this).after('<div class="invalid-feedback">File size must not exceed 5MB</div>'); //validating the file size to not exceed 5MB
            }
        } else {
            $(this).removeClass('is-invalid');
            $('.invalid-feedback').remove();
        }
    });

    
    $form.on('submit', function (e)
        // This section handles the submission of the claim form
    {
        const hours = parseFloat($hoursWorked.val());
        const rate = parseFloat($hourlyRate.val());

        // Clear any existing error messages so that no error messages repeat
        $('.invalid-feedback').remove();
        $('.is-invalid').removeClass('is-invalid');

        let isValid = true;

        // Validate the lecturer's hours worked
        if (!validateHours(hours)) {
            $hoursWorked.addClass('is-invalid');
            $hoursWorked.after('<div class="invalid-feedback">Please enter valid hours (1-200)</div>');
            isValid = false;
        }

        // Validate the lecturer's hourly rate
        if (!validateRate(rate)) {
            $hourlyRate.addClass('is-invalid');
            $hourlyRate.after('<div class="invalid-feedback">Please enter valid rate (1-1000)</div>');
            isValid = false;
        }

        // Validate file size so it does not exceed 5MB
        if (!validateFileSize()) {
            $fileInput.addClass('is-invalid');
            $fileInput.after('<div class="invalid-feedback">File size must not exceed 5MB</div>');
            isValid = false;
        }

        if (!isValid) {
            e.preventDefault(); // Prevent form submission if validation fails, aiding in error handling 
        }
    });
});