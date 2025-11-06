// account.js - JavaScript para las páginas de autenticación

document.addEventListener('DOMContentLoaded', function () {
    // Mejorar la experiencia con los inputs
    const inputs = document.querySelectorAll('.form-control');

    inputs.forEach(function (input) {
        // Efecto focus
        input.addEventListener('focus', function () {
            this.style.borderColor = 'var(--primary-color)';
            this.style.boxShadow = '0 0 0 3px rgba(212, 130, 92, 0.1)';
        });

        // Efecto blur
        input.addEventListener('blur', function () {
            this.style.borderColor = 'var(--secondary-color)';
            this.style.boxShadow = 'none';
        });
    });

    // Auto-ocultar alertas después de 5 segundos
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(function (alert) {
        setTimeout(function () {
            alert.style.transition = 'opacity 0.5s ease';
            alert.style.opacity = '0';
            setTimeout(function () {
                alert.remove();
            }, 500);
        }, 5000);
    });

    // Validación adicional del formulario de registro
    const registroForm = document.querySelector('form[action*="Registro"]');
    if (registroForm) {
        registroForm.addEventListener('submit', function (e) {
            const password = document.getElementById('Contrasenna');
            if (password && password.value.length < 6) {
                e.preventDefault();
                alert('La contraseña debe tener al menos 6 caracteres');
                password.focus();
            }
        });
    }

    // Validación de email en tiempo real
    const emailInputs = document.querySelectorAll('input[type="email"]');
    emailInputs.forEach(function (input) {
        input.addEventListener('blur', function () {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (this.value && !emailRegex.test(this.value)) {
                this.style.borderColor = '#c33';
            }
        });
    });
});