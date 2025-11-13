// ============================================
// USUARIO.JS - Funcionalidad de Usuarios
// ============================================

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN
    // ============================================
    var paginaActual = 1;
    var registrosPorPagina = 5;
    var todasLasFilas = [];

    var table = document.getElementById('laTablaDeUsuarios');
    if (table && table.getElementsByTagName('tbody')[0]) {
        todasLasFilas = Array.from(table.getElementsByTagName('tbody')[0].getElementsByTagName('tr'));
        if (todasLasFilas.length > 0 && !todasLasFilas[0].querySelector('td[colspan]')) {
            mostrarPagina(paginaActual);
        }
    }

    function mostrarPagina(pagina) {
        if (todasLasFilas.length === 0 || todasLasFilas[0].querySelector('td[colspan]')) {
            return;
        }

        var inicio = (pagina - 1) * registrosPorPagina;
        var fin = inicio + registrosPorPagina;

        todasLasFilas.forEach(function (row) {
            row.style.display = 'none';
        });

        for (var i = inicio; i < fin && i < todasLasFilas.length; i++) {
            todasLasFilas[i].style.display = '';
        }

        var totalRegistros = todasLasFilas.length;
        var registroInicio = inicio + 1;
        var registroFin = Math.min(fin, totalRegistros);

        var startRecord = document.getElementById('startRecord');
        var endRecord = document.getElementById('endRecord');
        var totalRecordsEl = document.getElementById('totalRecords');

        if (startRecord) startRecord.textContent = registroInicio;
        if (endRecord) endRecord.textContent = registroFin;
        if (totalRecordsEl) totalRecordsEl.textContent = totalRegistros;

        var btnAnterior = document.getElementById('btnAnterior');
        var btnSiguiente = document.getElementById('btnSiguiente');

        if (btnAnterior) {
            btnAnterior.disabled = pagina === 1;
            btnAnterior.style.opacity = pagina === 1 ? '0.5' : '1';
            btnAnterior.style.cursor = pagina === 1 ? 'not-allowed' : 'pointer';
        }

        if (btnSiguiente) {
            btnSiguiente.disabled = fin >= totalRegistros;
            btnSiguiente.style.opacity = fin >= totalRegistros ? '0.5' : '1';
            btnSiguiente.style.cursor = fin >= totalRegistros ? 'not-allowed' : 'pointer';
        }
    }

    // Funciones globales para los botones de paginación
    window.paginaAnterior = function () {
        if (paginaActual > 1) {
            paginaActual--;
            mostrarPagina(paginaActual);
        }
    };

    window.paginaSiguiente = function () {
        var totalPaginas = Math.ceil(todasLasFilas.length / registrosPorPagina);
        if (paginaActual < totalPaginas) {
            paginaActual++;
            mostrarPagina(paginaActual);
        }
    };

    // ============================================
    // BOTONES DE EDITAR
    // ============================================
    var botonesEditar = document.querySelectorAll('.btn-editar');
    botonesEditar.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var usuarioId = this.getAttribute('data-id');
            var editUrl = this.getAttribute('data-edit-url') || '/Usuario/EditarUsuario';
            window.location.href = editUrl + '?id=' + usuarioId;
        });
    });

    // ============================================
    // BOTONES DE VER DETALLES
    // ============================================
    var botonesDetalles = document.querySelectorAll('.btn-ver-detalles');
    botonesDetalles.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var fila = this.closest('tr');

            var detallesId = document.getElementById('detalles-id');
            var detallesNombre = document.getElementById('detalles-nombre');
            var detallesEmail = document.getElementById('detalles-email');
            var detallesRol = document.getElementById('detalles-rol');
            var estadoBadge = document.getElementById('detalles-estado-badge');

            if (detallesId) detallesId.textContent = fila.dataset.usuarioId;
            if (detallesNombre) detallesNombre.textContent = fila.dataset.usuarioNombre;
            if (detallesEmail) detallesEmail.textContent = fila.dataset.usuarioEmail;
            if (detallesRol) detallesRol.textContent = fila.dataset.usuarioRol;

            if (estadoBadge) {
                var estado = fila.dataset.usuarioEstado;
                estadoBadge.textContent = estado;
                estadoBadge.style.padding = '0.5rem 1rem';
                estadoBadge.style.borderRadius = '20px';
                estadoBadge.style.fontWeight = '600';
                estadoBadge.style.color = 'white';

                if (estado === 'Activo') {
                    estadoBadge.style.background = '#27ae60';
                } else {
                    estadoBadge.style.background = '#e74c3c';
                }
            }
        });
    });

    // ============================================
    // BOTONES DE ELIMINAR
    // ============================================
    var botonesEliminar = document.querySelectorAll('.btn-eliminar');
    botonesEliminar.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var fila = this.closest('tr');

            var eliminarId = document.getElementById('eliminar-id');
            var eliminarIdDisplay = document.getElementById('eliminar-id-display');
            var eliminarNombre = document.getElementById('eliminar-nombre');
            var eliminarEmail = document.getElementById('eliminar-email');
            var eliminarRol = document.getElementById('eliminar-rol');
            var eliminarEstadoBadge = document.getElementById('eliminar-estado-badge');

            if (eliminarId) eliminarId.value = fila.dataset.usuarioId;
            if (eliminarIdDisplay) eliminarIdDisplay.textContent = fila.dataset.usuarioId;
            if (eliminarNombre) eliminarNombre.textContent = fila.dataset.usuarioNombre;
            if (eliminarEmail) eliminarEmail.textContent = fila.dataset.usuarioEmail;
            if (eliminarRol) eliminarRol.textContent = fila.dataset.usuarioRol;

            if (eliminarEstadoBadge) {
                var estado = fila.dataset.usuarioEstado;
                eliminarEstadoBadge.textContent = estado;
                eliminarEstadoBadge.style.padding = '0.5rem 1rem';
                eliminarEstadoBadge.style.borderRadius = '20px';
                eliminarEstadoBadge.style.fontWeight = '600';
                eliminarEstadoBadge.style.color = 'white';

                if (estado === 'Activo') {
                    eliminarEstadoBadge.style.background = '#27ae60';
                } else {
                    eliminarEstadoBadge.style.background = '#e74c3c';
                }
            }

            var form = document.getElementById('formEliminarUsuario');
            var deleteUrl = btn.getAttribute('data-delete-url') || '/Usuario/EliminarUsuario';
            if (form) form.action = deleteUrl;
        });
    });

    // ============================================
    // BÚSQUEDA MEJORADA
    // ============================================
    var searchInput = document.getElementById('searchInput');
    if (searchInput) {
        searchInput.addEventListener('focus', function () {
            this.style.borderColor = 'var(--primary-color)';
            this.style.boxShadow = '0 4px 12px rgba(212, 130, 92, 0.2)';
        });
        searchInput.addEventListener('blur', function () {
            this.style.borderColor = 'var(--secondary-color)';
            this.style.boxShadow = 'none';
        });
    }

    // ============================================
    // FORMULARIOS - MEJORA DE INPUTS
    // ============================================
    var inputs = document.querySelectorAll('.form-control');
    inputs.forEach(function (input) {
        input.addEventListener('focus', function () {
            this.style.borderColor = 'var(--primary-color)';
            this.style.boxShadow = '0 0 0 3px rgba(212, 130, 92, 0.1)';
        });
        input.addEventListener('blur', function () {
            this.style.borderColor = 'var(--secondary-color)';
            this.style.boxShadow = 'none';
        });
    });

    // ============================================
    // VALIDACIÓN DE EMAIL EN TIEMPO REAL
    // ============================================
    var emailInput = document.querySelector('input[name="Email"]');
    if (emailInput) {
        emailInput.addEventListener('blur', function () {
            var email = this.value.trim();
            if (email && !validarEmail(email)) {
                this.style.borderColor = '#e74c3c';

                // Mostrar mensaje de error si no existe
                var errorMsg = this.parentElement.querySelector('.email-error-custom');
                if (!errorMsg) {
                    errorMsg = document.createElement('small');
                    errorMsg.className = 'email-error-custom';
                    errorMsg.style.color = '#c33';
                    errorMsg.style.fontSize = '0.9rem';
                    errorMsg.style.marginTop = '0.3rem';
                    errorMsg.style.display = 'block';
                    errorMsg.innerHTML = '<i class="fas fa-exclamation-circle"></i> Por favor, ingresa un email válido';
                    this.parentElement.appendChild(errorMsg);
                }
            } else {
                this.style.borderColor = 'var(--secondary-color)';

                // Eliminar mensaje de error si existe
                var errorMsg = this.parentElement.querySelector('.email-error-custom');
                if (errorMsg) {
                    errorMsg.remove();
                }
            }
        });
    }

    // ============================================
    // VALIDACIÓN DE CONTRASEÑA
    // ============================================
    var passwordInput = document.querySelector('input[name="Contrasenna"]');
    if (passwordInput) {
        // Indicador de fuerza de contraseña
        var strengthIndicator = document.createElement('div');
        strengthIndicator.id = 'password-strength';
        strengthIndicator.style.marginTop = '0.5rem';
        strengthIndicator.style.display = 'none';

        var strengthBar = document.createElement('div');
        strengthBar.style.height = '5px';
        strengthBar.style.borderRadius = '3px';
        strengthBar.style.transition = 'all 0.3s';
        strengthBar.style.marginBottom = '0.3rem';

        var strengthText = document.createElement('small');
        strengthText.style.fontSize = '0.85rem';
        strengthText.style.fontWeight = '500';

        strengthIndicator.appendChild(strengthBar);
        strengthIndicator.appendChild(strengthText);

        var infoSmall = passwordInput.parentElement.querySelector('small');
        if (infoSmall) {
            infoSmall.parentElement.insertBefore(strengthIndicator, infoSmall.nextSibling);
        } else {
            passwordInput.parentElement.appendChild(strengthIndicator);
        }

        passwordInput.addEventListener('input', function () {
            var password = this.value;

            if (password.length === 0) {
                strengthIndicator.style.display = 'none';
                return;
            }

            strengthIndicator.style.display = 'block';
            var strength = calcularFuerzaPassword(password);

            // Actualizar barra y texto según la fuerza
            switch (strength.nivel) {
                case 'débil':
                    strengthBar.style.width = '33%';
                    strengthBar.style.background = '#e74c3c';
                    strengthText.textContent = '🔴 Contraseña débil';
                    strengthText.style.color = '#e74c3c';
                    break;
                case 'media':
                    strengthBar.style.width = '66%';
                    strengthBar.style.background = '#f39c12';
                    strengthText.textContent = '🟡 Contraseña media';
                    strengthText.style.color = '#f39c12';
                    break;
                case 'fuerte':
                    strengthBar.style.width = '100%';
                    strengthBar.style.background = '#27ae60';
                    strengthText.textContent = '🟢 Contraseña fuerte';
                    strengthText.style.color = '#27ae60';
                    break;
            }
        });
    }

    // ============================================
    // VALIDACIÓN DE SELECCIÓN DE ROL
    // ============================================
    var rolSelect = document.querySelector('select[name="IdRol"]');
    if (rolSelect) {
        rolSelect.addEventListener('change', function () {
            if (this.value === '') {
                this.style.borderColor = '#e74c3c';
            } else {
                this.style.borderColor = 'var(--primary-color)';
            }
        });
    }

    // ============================================
    // HOVER EN FILAS DE TABLA
    // ============================================
    var filasTabla = document.querySelectorAll('#laTablaDeUsuarios tbody tr');
    filasTabla.forEach(function (fila) {
        // Solo aplicar hover si no es la fila de "no hay datos"
        if (!fila.querySelector('td[colspan]')) {
            fila.addEventListener('mouseenter', function () {
                this.style.background = 'var(--light-color)';
            });
            fila.addEventListener('mouseleave', function () {
                this.style.background = 'transparent';
            });
        }
    });

    // ============================================
    // AUTO-CIERRE DE ALERTAS
    // ============================================
    var alertas = document.querySelectorAll('.alert');
    alertas.forEach(function (alerta) {
        // Auto-cerrar después de 5 segundos
        setTimeout(function () {
            var closeButton = alerta.querySelector('.btn-close');
            if (closeButton) {
                closeButton.click();
            }
        }, 5000);
    });
});

// ============================================
// FUNCIÓN AUXILIAR: VALIDAR EMAIL
// ============================================
function validarEmail(email) {
    var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return regex.test(email);
}

// ============================================
// FUNCIÓN AUXILIAR: CALCULAR FUERZA DE CONTRASEÑA
// ============================================
function calcularFuerzaPassword(password) {
    var fuerza = 0;

    // Longitud
    if (password.length >= 8) fuerza += 1;
    if (password.length >= 12) fuerza += 1;

    // Contiene minúsculas
    if (/[a-z]/.test(password)) fuerza += 1;

    // Contiene mayúsculas
    if (/[A-Z]/.test(password)) fuerza += 1;

    // Contiene números
    if (/[0-9]/.test(password)) fuerza += 1;

    // Contiene caracteres especiales
    if (/[^a-zA-Z0-9]/.test(password)) fuerza += 1;

    var nivel = 'débil';
    if (fuerza >= 4) nivel = 'fuerte';
    else if (fuerza >= 3) nivel = 'media';

    return { nivel: nivel, puntos: fuerza };
}

// ============================================
// FUNCIÓN AUXILIAR: VALIDAR FORMULARIO
// ============================================
function validarFormularioUsuario(form) {
    var esValido = true;
    var errores = [];

    // Validar nombre
    var nombreInput = form.querySelector('input[name="NombreUsuario"]');
    if (nombreInput && nombreInput.value.trim() === '') {
        esValido = false;
        errores.push('El nombre del usuario es requerido');
    }

    // Validar email
    var emailInput = form.querySelector('input[name="Email"]');
    if (emailInput) {
        var email = emailInput.value.trim();
        if (email === '') {
            esValido = false;
            errores.push('El email es requerido');
        } else if (!validarEmail(email)) {
            esValido = false;
            errores.push('El email no es válido');
        }
    }

    // Validar contraseña (solo en crear, no en editar)
    var passwordInput = form.querySelector('input[name="Contrasenna"]');
    var esEdicion = form.action.indexOf('Editar') > -1;

    if (passwordInput && !esEdicion) {
        var password = passwordInput.value;
        if (password === '') {
            esValido = false;
            errores.push('La contraseña es requerida');
        } else if (password.length < 6) {
            esValido = false;
            errores.push('La contraseña debe tener al menos 6 caracteres');
        }
    }

    // Validar rol
    var rolSelect = form.querySelector('select[name="IdRol"]');
    if (rolSelect && rolSelect.value === '') {
        esValido = false;
        errores.push('Debe seleccionar un rol');
    }

    return { esValido: esValido, errores: errores };
}