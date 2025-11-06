// auditorias.js - JavaScript para la gestión de auditorías

// Variables de paginación
var paginaActual = 1;
var registrosPorPagina = 5;
var todasLasFilas = [];

document.addEventListener('DOMContentLoaded', function () {
    inicializarPaginacion();
    inicializarBotonesDetalles();
    inicializarBusqueda();
});

// Inicializar paginación
function inicializarPaginacion() {
    var table = document.getElementById('laTablaDeAuditorias');
    if (table && table.getElementsByTagName('tbody')[0]) {
        todasLasFilas = Array.from(table.getElementsByTagName('tbody')[0].getElementsByTagName('tr'));
        if (todasLasFilas.length > 0 && !todasLasFilas[0].querySelector('td[colspan]')) {
            mostrarPagina(paginaActual);
        }
    }
}

// Inicializar botones de ver detalles
function inicializarBotonesDetalles() {
    var botonesDetalles = document.querySelectorAll('.btn-ver-detalles');
    botonesDetalles.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var fila = this.closest('tr');
            cargarDetallesAuditoria(fila);
        });
    });
}

// Cargar detalles de auditoría en el modal
function cargarDetallesAuditoria(fila) {
    // Información básica
    document.getElementById('detalles-id').textContent = fila.dataset.auditoriaId;
    document.getElementById('detalles-tabla').textContent = fila.dataset.auditoriaTabla;
    document.getElementById('detalles-id-registro').textContent = fila.dataset.auditoriaIdRegistro;
    document.getElementById('detalles-accion').textContent = fila.dataset.auditoriaAccion;
    document.getElementById('detalles-usuario').textContent = fila.dataset.auditoriaUsuario;
    document.getElementById('detalles-fecha').textContent = fila.dataset.auditoriaFecha;
    document.getElementById('detalles-descripcion').textContent = fila.dataset.auditoriaDescripcion || 'Sin descripción';

    // Configurar badge de acción
    configurarBadgeAccion(fila.dataset.auditoriaAccion);

    // Cargar valores anteriores y nuevos
    cargarValoresAnteriores(fila.dataset.auditoriaValoresAnteriores);
    cargarValoresNuevos(fila.dataset.auditoriaValoresNuevos);
}

// Configurar badge de acción con color correspondiente
function configurarBadgeAccion(accion) {
    var accionBadge = document.getElementById('detalles-accion-badge');
    accionBadge.textContent = accion;

    var badgeColor = '#95a5a6';
    if (accion.includes('Creación')) badgeColor = '#27ae60';
    else if (accion.includes('Actualización')) badgeColor = '#3498db';
    else if (accion.includes('Eliminación')) badgeColor = '#e74c3c';

    accionBadge.style.background = badgeColor;
    accionBadge.style.color = 'white';
    accionBadge.style.padding = '0.5rem 1rem';
    accionBadge.style.borderRadius = '20px';
}

// Cargar valores anteriores
function cargarValoresAnteriores(valoresAnteriores) {
    var antDiv = document.getElementById('detalles-valores-anteriores');
    var section = document.getElementById('section-valores-anteriores');

    if (valoresAnteriores && valoresAnteriores !== 'null' && valoresAnteriores !== '') {
        try {
            var jsonFormatted = JSON.stringify(JSON.parse(valoresAnteriores), null, 2);
            antDiv.textContent = jsonFormatted;
        } catch {
            antDiv.textContent = valoresAnteriores;
        }
        section.style.display = 'block';
    } else {
        section.style.display = 'none';
    }
}

// Cargar valores nuevos
function cargarValoresNuevos(valoresNuevos) {
    var nuevDiv = document.getElementById('detalles-valores-nuevos');
    var section = document.getElementById('section-valores-nuevos');

    if (valoresNuevos && valoresNuevos !== 'null' && valoresNuevos !== '') {
        try {
            var jsonFormatted = JSON.stringify(JSON.parse(valoresNuevos), null, 2);
            nuevDiv.textContent = jsonFormatted;
        } catch {
            nuevDiv.textContent = valoresNuevos;
        }
        section.style.display = 'block';
    } else {
        section.style.display = 'none';
    }
}

// Inicializar funcionalidad de búsqueda
function inicializarBusqueda() {
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
}

// Mostrar página específica
function mostrarPagina(pagina) {
    if (todasLasFilas.length === 0 || todasLasFilas[0].querySelector('td[colspan]')) {
        return;
    }

    var inicio = (pagina - 1) * registrosPorPagina;
    var fin = inicio + registrosPorPagina;

    // Ocultar todas las filas
    todasLasFilas.forEach(function (row) {
        row.style.display = 'none';
    });

    // Mostrar filas de la página actual
    for (var i = inicio; i < fin && i < todasLasFilas.length; i++) {
        todasLasFilas[i].style.display = '';
    }

    actualizarInfoPaginacion(inicio, fin);
    actualizarEstadoBotones(pagina, fin);
}

// Actualizar información de paginación
function actualizarInfoPaginacion(inicio, fin) {
    var totalRegistros = todasLasFilas.length;
    var registroInicio = inicio + 1;
    var registroFin = Math.min(fin, totalRegistros);

    document.getElementById('startRecord').textContent = registroInicio;
    document.getElementById('endRecord').textContent = registroFin;
    document.getElementById('totalRecords').textContent = totalRegistros;
}

// Actualizar estado de los botones de paginación
function actualizarEstadoBotones(pagina, fin) {
    var btnAnterior = document.getElementById('btnAnterior');
    var btnSiguiente = document.getElementById('btnSiguiente');

    // Actualizar botón Anterior
    btnAnterior.disabled = pagina === 1;
    if (pagina === 1) {
        btnAnterior.style.opacity = '0.5';
        btnAnterior.style.cursor = 'not-allowed';
    } else {
        btnAnterior.style.opacity = '1';
        btnAnterior.style.cursor = 'pointer';
    }

    // Actualizar botón Siguiente
    btnSiguiente.disabled = fin >= todasLasFilas.length;
    if (fin >= todasLasFilas.length) {
        btnSiguiente.style.opacity = '0.5';
        btnSiguiente.style.cursor = 'not-allowed';
    } else {
        btnSiguiente.style.opacity = '1';
        btnSiguiente.style.cursor = 'pointer';
    }
}

// Ir a página anterior
function paginaAnterior() {
    if (paginaActual > 1) {
        paginaActual--;
        mostrarPagina(paginaActual);
    }
}

// Ir a página siguiente
function paginaSiguiente() {
    var totalPaginas = Math.ceil(todasLasFilas.length / registrosPorPagina);
    if (paginaActual < totalPaginas) {
        paginaActual++;
        mostrarPagina(paginaActual);
    }
}