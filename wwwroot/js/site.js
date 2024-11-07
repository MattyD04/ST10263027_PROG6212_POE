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

    function validateHours(hours) {
        return hours > 0; // Only checking that hours is positive
    }
    function validateRate(rate) {
        return rate > 0; // Only checking that rate is positive
    }
    function validateFileSize() {
        //function to validate the size of a file
        const fileInput = $fileInput[0];
        if (fileInput.files.length > 0) {
            const fileSize = fileInput.files[0].size / 1024 / 1024; // Convert to MB
            return fileSize <= 5; // Max 5MB for file size
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
            $hoursWorked.after('<div class="invalid-feedback">Please enter a positive number of hours</div>');
            isValid = false;
        }
        // Validate the lecturer's hourly rate
        if (!validateRate(rate)) {
            $hourlyRate.addClass('is-invalid');
            $hourlyRate.after('<div class="invalid-feedback">Please enter a positive rate</div>');
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
    // Claim Submission Form Functionality
    $(document).ready(function () {
        initializeClaimForm();
        initializeClaimsVerificationPage();
    });

    // Claim Form Functions
    function initializeClaimForm() {
        // Cache DOM elements
        const $hoursWorked = $('input[name="hoursWorked"]');
        const $hourlyRate = $('input[name="hourlyRate"]');
        const $form = $('form');
        const $fileInput = $('input[type="file"]');
        const $totalAmountDisplay = $('.total-amount-display');

        // Calculate and display total amount
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
            return hours > 0;
        }

        function validateRate(rate) {
            return rate > 0;
        }

        function validateFileSize() {
            const fileInput = $fileInput[0];
            if (fileInput.files.length > 0) {
                const fileSize = fileInput.files[0].size / 1024 / 1024; // Convert to MB
                return fileSize <= 5;
            }
            return true;
        }

        // Event listeners
        $hoursWorked.on('input', calculateTotal);
        $hourlyRate.on('input', calculateTotal);

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

        $form.on('submit', function (e) {
            const hours = parseFloat($hoursWorked.val());
            const rate = parseFloat($hourlyRate.val());

            $('.invalid-feedback').remove();
            $('.is-invalid').removeClass('is-invalid');

            let isValid = true;

            if (!validateHours(hours)) {
                $hoursWorked.addClass('is-invalid');
                $hoursWorked.after('<div class="invalid-feedback">Please enter a positive number of hours</div>');
                isValid = false;
            }

            if (!validateRate(rate)) {
                $hourlyRate.addClass('is-invalid');
                $hourlyRate.after('<div class="invalid-feedback">Please enter a positive rate</div>');
                isValid = false;
            }

            if (!validateFileSize()) {
                $fileInput.addClass('is-invalid');
                $fileInput.after('<div class="invalid-feedback">File size must not exceed 5MB</div>');
                isValid = false;
            }

            if (!isValid) {
                e.preventDefault();
            }
        });
    }

    // Claims Verification Page Functions
    function initializeClaimsVerificationPage() {
        if (!document.querySelector('.claims-table')) return;

        loadPendingClaims();
    }

    function loadPendingClaims() {
        $.ajax({
            url: '/api/VerifyClaimsWebAPI/GetPendingClaims',
            method: 'GET',
            success: function (claims) {
                updateClaimsTable(claims);
                updateClaimsCount(claims.length);
            },
            error: function (xhr, status, error) {
                console.error('Error loading claims:', error);
                showAlert('Error loading claims. Please try again.', 'danger');
            }
        });
    }

    function updateClaimsTable(claims) {
        const $tableBody = $('.claims-table tbody');
        if (!$tableBody.length) return;

        $tableBody.empty();

        if (claims.length === 0) {
            $tableBody.html(`
            <tr>
                <td colspan="9" class="no-claims-message">No pending claims to display.</td>
            </tr>`);
            return;
        }

        claims.forEach(claim => {
            const row = `
            <tr>
                <td>${claim.claimNum}</td>
                <td>${claim.lecturerNum}</td>
                <td>${new Date(claim.submissionDate).toLocaleDateString()}</td>
                <td>${claim.hoursWorked}</td>
                <td>R${claim.hourlyRate.toFixed(2)}</td>
                <td>R${claim.totalAmount.toFixed(2)}</td>
                <td class="notes-cell" title="${claim.comments}">${claim.comments}</td>
                <td>${claim.filename || 'No file'}</td>
                <td class="action-buttons">
                    <button onclick="handleClaimAction(${claim.claimID}, 'Approve')" class="accept-btn">Approve</button>
                    <button onclick="handleClaimAction(${claim.claimID}, 'Reject')" class="reject-btn">Reject</button>
                </td>
            </tr>`;
            $tableBody.append(row);
        });
    }

    function updateClaimsCount(count) {
        $('.claims-count').text(`Claims found: ${count}`);
    }

    function handleClaimAction(claimId, action) {
        if (!confirm(`Are you sure you want to ${action.toLowerCase()} this claim?`)) {
            return;
        }

        $.ajax({
            url: `/api/VerifyClaimsWebAPI/${action}Claim/${claimId}`,
            method: 'PUT',
            contentType: 'application/json',
            success: function (data) {
                showAlert(data.message, 'success');
                loadPendingClaims();
            },
            error: function (xhr, status, error) {
                console.error(`Error ${action.toLowerCase()}ing claim:`, error);
                showAlert(`Error ${action.toLowerCase()}ing claim. Please try again.`, 'danger');
            }
        });
    }

    function showAlert(message, type) {
        const $alertDiv = $(`<div class="alert alert-${type}">${message}</div>`);
        const $container = $('.form-container');

        if ($container.length) {
            $container.prepend($alertDiv);
            setTimeout(() => $alertDiv.remove(), 5000);
        }
    }
}); 
   