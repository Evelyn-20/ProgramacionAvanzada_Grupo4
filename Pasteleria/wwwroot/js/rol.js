// ============================================
// ROL.JS - Funcionalidad de Roles
// ============================================

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN
    // ============================================
    var paginaActual = 1;
    var registrosPorPagina = 10;
    var todasLasFilas = [];

    var table = document.getElementById('laTablaDeRoles');
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
            var rolId = this.getAttribute('data-id');
            var editUrl = this.getAttribute('data-edit-url') || '/Rol/EditarRol';
            window.location.href = editUrl + '?id=' + rolId;
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
            var estadoBadge = document.getElementById('detalles-estado-badge');

            if (detallesId) detallesId.textContent = fila.dataset.rolId;

            if (detallesNombre) {
                // Limpiar el contenido previo y actualizar
                var spanElement = detallesNombre.querySelector('span');
                if (spanElement) {
                    spanElement.textContent = fila.dataset.rolNombre;
                } else {
                    // Si no existe el span, actualizar el texto completo manteniendo el icono
                    detallesNombre.innerHTML = '<i class="fas fa-user-tag" style="color: var(--primary-color);"></i> <span>' + fila.dataset.rolNombre + '</span>';
                }
            }

            if (estadoBadge) {
                var estado = fila.dataset.rolEstado;
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
            var eliminarEstadoBadge = document.getElementById('eliminar-estado-badge');

            if (eliminarId) eliminarId.value = fila.dataset.rolId;
            if (eliminarIdDisplay) eliminarIdDisplay.textContent = fila.dataset.rolId;
            if (eliminarNombre) eliminarNombre.textContent = fila.dataset.rolNombre;

            if (eliminarEstadoBadge) {
                var estado = fila.dataset.rolEstado;
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

            var form = document.getElementById('formEliminarRol');
            var deleteUrl = btn.getAttribute('data-delete-url') || '/Rol/EliminarRol';
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
    // VALIDACIÓN DE NOMBRE DE ROL
    // ============================================
    var nombreRolInput = document.querySelector('input[name="NombreRol"]');
    if (nombreRolInput) {
        nombreRolInput.addEventListener('input', function () {
            // Convertir la primera letra de cada palabra en mayúscula
            var palabras = this.value.split(' ');
            var palabrasCapitalizadas = palabras.map(function (palabra) {
                if (palabra.length > 0) {
                    return palabra.charAt(0).toUpperCase() + palabra.slice(1).toLowerCase();
                }
                return palabra;
            });

            var cursorPos = this.selectionStart;
            this.value = palabrasCapitalizadas.join(' ');
            this.setSelectionRange(cursorPos, cursorPos);
        });

        // Validar que no esté vacío al enviar
        var form = nombreRolInput.closest('form');
        if (form) {
            form.addEventListener('submit', function (e) {
                var valor = nombreRolInput.value.trim();
                if (valor === '') {
                    e.preventDefault();
                    alert('Por favor, ingresa un nombre para el rol.');
                    nombreRolInput.focus();
                    return false;
                }
            });
        }
    }

    // ============================================
    // HOVER EN FILAS DE TABLA
    // ============================================
    var filasTabla = document.querySelectorAll('#laTablaDeRoles tbody tr');
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
    // ANIMACIÓN DE GUARDADO EXITOSO
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
// FUNCIÓN AUXILIAR: VALIDAR NOMBRE DE ROL
// ============================================
function validarNombreRol(nombre) {
    // Eliminar espacios en blanco al inicio y final
    nombre = nombre.trim();

    // Validar que no esté vacío
    if (nombre === '') {
        return { valido: false, mensaje: 'El nombre del rol no puede estar vacío' };
    }

    // Validar longitud mínima
    if (nombre.length < 3) {
        return { valido: false, mensaje: 'El nombre del rol debe tener al menos 3 caracteres' };
    }

    // Validar longitud máxima
    if (nombre.length > 50) {
        return { valido: false, mensaje: 'El nombre del rol no puede exceder 50 caracteres' };
    }

    // Validar que solo contenga letras, números y espacios
    var regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9\s]+$/;
    if (!regex.test(nombre)) {
        return { valido: false, mensaje: 'El nombre del rol solo puede contener letras, números y espacios' };
    }

    return { valido: true };
}

// ============================================
// FUNCIÓN AUXILIAR: CAPITALIZAR TEXTO
// ============================================
function capitalizarTexto(texto) {
    return texto.split(' ').map(function (palabra) {
        if (palabra.length > 0) {
            return palabra.charAt(0).toUpperCase() + palabra.slice(1).toLowerCase();
        }
        return palabra;
    }).join(' ');
}