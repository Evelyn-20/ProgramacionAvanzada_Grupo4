// ============================================
// PEDIDO.JS - Funcionalidad de Pedidos
// ============================================

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN
    // ============================================
    var paginaActual = 1;
    var registrosPorPagina = 10;
    var todasLasFilas = [];

    var table = document.getElementById('laTablaDePedidos');
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
    // BOTONES DE DETALLES
    // ============================================
    var botonesDetalles = document.querySelectorAll('.btn-detalles');
    botonesDetalles.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var pedidoId = this.getAttribute('data-id');
            var cliente = this.getAttribute('data-cliente');
            var fecha = this.getAttribute('data-fecha');
            var total = this.getAttribute('data-total');
            var estado = this.getAttribute('data-estado');

            // Actualizar contenido del modal
            var detallesPedidoId = document.getElementById('detalles-pedido-id');
            var detallesPedidoCliente = document.getElementById('detalles-pedido-cliente');
            var detallesPedidoFecha = document.getElementById('detalles-pedido-fecha');
            var detallesPedidoTotal = document.getElementById('detalles-pedido-total');
            var estadoBadge = document.getElementById('detalles-pedido-estado-badge');

            if (detallesPedidoId) detallesPedidoId.textContent = pedidoId;
            if (detallesPedidoCliente) detallesPedidoCliente.textContent = cliente;
            if (detallesPedidoFecha) detallesPedidoFecha.textContent = fecha;
            if (detallesPedidoTotal) detallesPedidoTotal.textContent = '₡' + total;

            // Actualizar badge de estado
            if (estadoBadge) {
                estadoBadge.textContent = estado;
                var estadoColor = obtenerColorEstado(estado);
                estadoBadge.style.background = estadoColor;
                estadoBadge.style.color = 'white';
                estadoBadge.style.padding = '0.5rem 1rem';
                estadoBadge.style.borderRadius = '20px';
                estadoBadge.style.display = 'inline-block';
                estadoBadge.style.fontWeight = '600';
            }
        });
    });

    // ============================================
    // BOTONES DE EDITAR ESTADO
    // ============================================
    var botonesEditarEstado = document.querySelectorAll('.btn-editar-estado');
    botonesEditarEstado.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var pedidoId = this.getAttribute('data-id');
            var estadoActual = this.getAttribute('data-estado');

            var editarEstadoId = document.getElementById('editar-estado-id');
            var editarEstadoSelect = document.getElementById('editar-estado-select');

            if (editarEstadoId) editarEstadoId.value = pedidoId;
            if (editarEstadoSelect) editarEstadoSelect.value = estadoActual;
        });
    });

    // ============================================
    // BÚSQUEDA MEJORADA
    // ============================================
    var searchInput = document.getElementById('buscar');
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
    // SELECT DE ESTADO - MEJORA VISUAL
    // ============================================
    var selectEstado = document.getElementById('editar-estado-select');
    if (selectEstado) {
        selectEstado.addEventListener('focus', function () {
            this.style.borderColor = 'var(--primary-color)';
            this.style.boxShadow = '0 0 0 3px rgba(212, 130, 92, 0.1)';
        });
        selectEstado.addEventListener('blur', function () {
            this.style.borderColor = 'var(--secondary-color)';
            this.style.boxShadow = 'none';
        });
    }

    // ============================================
    // HOVER EN FILAS DE TABLA
    // ============================================
    var filasTabla = document.querySelectorAll('#laTablaDePedidos tbody tr');
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

// ============================================
// FUNCIÓN AUXILIAR: OBTENER COLOR POR ESTADO
// ============================================
function obtenerColorEstado(estado) {
    switch (estado) {
        case 'Pendiente':
            return '#f39c12';
        case 'En Proceso':
            return '#3498db';
        case 'Completado':
            return '#27ae60';
        case 'Cancelado':
            return '#e74c3c';
        default:
            return '#95a5a6';
    }
}

// ============================================
// FUNCIÓN AUXILIAR: OBTENER ÍCONO POR ESTADO
// ============================================
function obtenerIconoEstado(estado) {
    switch (estado) {
        case 'Pendiente':
            return 'fa-clock';
        case 'En Proceso':
            return 'fa-spinner';
        case 'Completado':
            return 'fa-check-circle';
        case 'Cancelado':
            return 'fa-times-circle';
        default:
            return 'fa-question-circle';
    }
}