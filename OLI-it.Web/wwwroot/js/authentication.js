/**
 * Authentication module for handling login and logout functionality
 */

/**
 * Handles login form submission
 * @param {Event} event - The form submit event
 */
async function handleLogin(event) {
    event.preventDefault();

    const form = event.target;
    const stammName = form.stammName.value;
    const unterschrift = form.unterschrift.value;
    const errorSpan = document.getElementById('loginError');

    try {
        const formData = new FormData();
        formData.append('StammName', stammName);
        formData.append('Unterschrift', unterschrift);

        // Get anti-forgery token
        const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

        const response = await fetch('/api/login', {
            method: 'POST',
            headers: {
                'X-CSRF-TOKEN': token
            },
            body: formData
        });

        // Read the response body regardless of status
        const text = await response.text();

        if (!response.ok) {
            errorSpan.textContent = `Server error (${response.status}). Check console.`;
            errorSpan.style.display = 'inline-block';
            setTimeout(() => {
                errorSpan.style.display = 'none';
            }, 5000);
            return;
        }

        // Try to parse as JSON
        let result;
        try {
            result = JSON.parse(text);
        } catch (e) {
            console.error('Failed to parse JSON:', e);
            throw new Error('Server returned non-JSON response');
        }

        if (result.success) {
            // Redirect to stammcard if StammGuid is provided
            if (result.stammGuid) {
                window.location.href = `/Stamm/${result.stammGuid}`;
            } else {
                // Fallback: Reload the page to show authenticated state
                window.location.reload();
            }
        } else {
            // Show error message
            errorSpan.textContent = result.message || 'Login failed';
            errorSpan.style.display = 'inline-block';
            setTimeout(() => {
                errorSpan.style.display = 'none';
            }, 5000);
        }
    } catch (error) {
        console.error('Login error:', error);
        errorSpan.textContent = 'An error occurred. Check console for details.';
        errorSpan.style.display = 'inline-block';
        setTimeout(() => {
            errorSpan.style.display = 'none';
        }, 5000);
    }
}

/**
 * Handles logout button click
 * @param {Event} event - The button click event
 */
async function handleLogout(event) {
    event.preventDefault();

    try {
        // Get anti-forgery token
        const token = document.querySelector('#hiddenAntiForgeryForm input[name="__RequestVerificationToken"]').value;

        const response = await fetch('/api/logout', {
            method: 'POST',
            headers: {
                'X-CSRF-TOKEN': token
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const result = await response.json();

        if (result.success) {
            // Reload the page to show logged out state
            window.location.reload();
        } else {
            // Reload anyway
            window.location.reload();
        }
    } catch (error) {
        console.error('Logout error:', error);
        // Reload anyway
        window.location.reload();
    }
}
