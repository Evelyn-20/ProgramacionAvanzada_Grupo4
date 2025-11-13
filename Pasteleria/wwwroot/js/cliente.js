// ============================================
// CLIENTE.JS - Funcionalidad de Clientes
// ============================================

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN
    // ============================================
    var paginaActual = 1;
    var registrosPorPagina = 5;
    var todasLasFilas = [];

    var table = document.getElementById('laTablaDeClientes');
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
            var clienteId = this.getAttribute('data-id');
            var editUrl = this.getAttribute('data-edit-url') || '/Cliente/EditarCliente';
            window.location.href = editUrl + '?id=' + clienteId;
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
            var detallesCedula = document.getElementById('detalles-cedula');
            var detallesNombre = document.getElementById('detalles-nombre');
            var detallesCorreo = document.getElementById('detalles-correo');
            var detallesTelefono = document.getElementById('detalles-telefono');
            var detallesDireccion = document.getElementById('detalles-direccion');
            var estadoBadge = document.getElementById('detalles-estado-badge');

            if (detallesId) detallesId.textContent = fila.dataset.clienteId;
            if (detallesCedula) detallesCedula.textContent = fila.dataset.clienteCedula;
            if (detallesNombre) detallesNombre.textContent = fila.dataset.clienteNombre;
            if (detallesCorreo) detallesCorreo.textContent = fila.dataset.clienteCorreo;
            if (detallesTelefono) detallesTelefono.textContent = fila.dataset.clienteTelefono;
            if (detallesDireccion) detallesDireccion.textContent = fila.dataset.clienteDireccion || 'Sin dirección';

            if (estadoBadge) {
                var estado = fila.dataset.clienteEstado;
                estadoBadge.textContent = estado;
                estadoBadge.style.padding = '0.5rem 1rem';
                estadoBadge.style.borderRadius = '20px';

                if (estado === 'Activo') {
                    estadoBadge.style.background = '#27ae60';
                    estadoBadge.style.color = 'white';
                } else {
                    estadoBadge.style.background = '#e74c3c';
                    estadoBadge.style.color = 'white';
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
            var eliminarCedula = document.getElementById('eliminar-cedula');
            var eliminarCorreo = document.getElementById('eliminar-correo');
            var eliminarTelefono = document.getElementById('eliminar-telefono');
            var eliminarEstadoBadge = document.getElementById('eliminar-estado-badge');

            if (eliminarId) eliminarId.value = fila.dataset.clienteId;
            if (eliminarIdDisplay) eliminarIdDisplay.textContent = fila.dataset.clienteId;
            if (eliminarNombre) eliminarNombre.textContent = fila.dataset.clienteNombre;
            if (eliminarCedula) eliminarCedula.textContent = fila.dataset.clienteCedula;
            if (eliminarCorreo) eliminarCorreo.textContent = fila.dataset.clienteCorreo;
            if (eliminarTelefono) eliminarTelefono.textContent = fila.dataset.clienteTelefono;

            if (eliminarEstadoBadge) {
                var estado = fila.dataset.clienteEstado;
                eliminarEstadoBadge.textContent = estado;
                eliminarEstadoBadge.style.padding = '0.5rem 1rem';
                eliminarEstadoBadge.style.borderRadius = '20px';

                if (estado === 'Activo') {
                    eliminarEstadoBadge.style.background = '#27ae60';
                    eliminarEstadoBadge.style.color = 'white';
                } else {
                    eliminarEstadoBadge.style.background = '#e74c3c';
                    eliminarEstadoBadge.style.color = 'white';
                }
            }

            var form = document.getElementById('formEliminarCliente');
            var deleteUrl = btn.getAttribute('data-delete-url') || '/Cliente/EliminarCliente';
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
    // HOVER EN FILAS DE TABLA
    // ============================================
    var filasTabla = document.querySelectorAll('#laTablaDeClientes tbody tr');
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
});