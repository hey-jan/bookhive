function showApiFeedback(message, type) {
    const feedback = document.getElementById("api-feedback");
    if (!feedback) {
        return;
    }

    feedback.className = `alert alert-${type} mb-4`;
    feedback.textContent = message;
    feedback.hidden = false;
}

async function handleApiCartFormSubmit(form, event) {
    event.preventDefault();

    const endpoint = form.dataset.apiEndpoint;
    const method = (form.dataset.apiMethod || "POST").toUpperCase();

    try {
        let response;

        if (method === "PUT") {
            const quantity = Number(form.querySelector("[name='quantity']")?.value ?? 0);
            const cartItemId = form.querySelector("[name='cartItemId']")?.value;

            response = await fetch(`${endpoint}/${cartItemId}`, {
                method,
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ quantity })
            });
        } else if (method === "DELETE") {
            const cartItemId = form.querySelector("[name='cartItemId']")?.value;

            response = await fetch(`${endpoint}/${cartItemId}`, {
                method
            });
        } else {
            response = await fetch(endpoint, {
                method,
                body: new FormData(form)
            });
        }

        const payload = await response.json();

        if (!response.ok) {
            showApiFeedback(payload.message || "The request could not be completed.", "danger");
            return;
        }

        showApiFeedback(payload.message || "Request completed.", "success");

        if (form.dataset.reloadOnSuccess === "true") {
            window.location.reload();
        }
    } catch (error) {
        showApiFeedback("A network error prevented the request from completing.", "danger");
    }
}

document.addEventListener("DOMContentLoaded", () => {
    const cartForms = document.querySelectorAll("[data-api-cart-form]");

    cartForms.forEach(form => {
        form.addEventListener("submit", event => handleApiCartFormSubmit(form, event));
    });
});
