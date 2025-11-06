// categorias.js - JavaScript para la gestión de categorías

// Variables de paginación
var paginaActual = 1;
var registrosPorPagina = 5;
var todasLasFilas = [];

document.addEventListener('DOMContentLoaded', function () {
    inicializarCategorias();
});

// Inicializar funcionalidades según la página
function inicializarCategorias() {
    const paginaActual = obtenerPaginaActual();

    switch (paginaActual) {
        case 'listado':
            inicializarListadoCategorias();
            break;
        case 'crear':
        case 'editar':
            inicializarFormularioCategoria();
            break;
    }
}

// Detectar página actual
function obtenerPaginaActual() {
    if (document.getElementById('laTablaDeCategor')) {
        return 'listado';
    } else if (document.getElementById('archivoImagen')) {
        return document.querySelector('input[name="IdCategoria"]') ? 'editar' : 'crear';
    }
    return 'unknown';
}

// ========================================
// LISTADO DE CATEGORÍAS
// ========================================
function inicializarListadoCategorias() {
    inicializarPaginacion();
    inicializarBotonesListado();
    inicializarBusqueda();
}

// Inicializar paginación
function inicializarPaginacion() {
    var table = document.getElementById('laTablaDeCategor');
    if (table && table.getElementsByTagName('tbody')[0]) {
        todasLasFilas = Array.from(table.getElementsByTagName('tbody')[0].getElementsByTagName('tr'));
        if (todasLasFilas.length > 0 && !todasLasFilas[0].querySelector('td[colspan]')) {
            mostrarPagina(paginaActual);
        }
    }
}

// Inicializar botones del listado
function inicializarBotonesListado() {
    // Botones de editar
    var botonesEditar = document.querySelectorAll('.btn-editar');
    botonesEditar.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var categoriaId = this.getAttribute('data-id');
            window.location.href = '/Categoria/EditarCategoria?id=' + categoriaId;
        });
    });

    // Botones de detalles
    var botonesDetalles = document.querySelectorAll('.btn-detalles');
    botonesDetalles.forEach(function (btn) {
        btn.addEventListener('click', function () {
            cargarDetallesCategoria(this);
        });
    });

    // Botones de eliminar
    var botonesEliminar = document.querySelectorAll('.btn-eliminar');
    botonesEliminar.forEach(function (btn) {
        btn.addEventListener('click', function () {
            cargarModalEliminar(this);
        });
    });
}

// Cargar detalles de categoría en modal
function cargarDetallesCategoria(boton) {
    var categoriaId = boton.getAttribute('data-id');

    document.getElementById('detalles-id').textContent = categoriaId;
    document.getElementById('detalles-nombre').textContent = boton.getAttribute('data-nombre');

    // Cargar imagen
    var imagenElement = document.getElementById('detalles-imagen');
    imagenElement.src = '/Categoria/ObtenerImagenCategoria?id=' + categoriaId;
    imagenElement.alt = boton.getAttribute('data-nombre');

    // Estado
    var estado = boton.getAttribute('data-estado');
    var estadoBadge = document.getElementById('detalles-estado-badge');
    estadoBadge.textContent = estado;
    estadoBadge.style.background = estado === 'Activo' ? '#27ae60' : '#e74c3c';
    estadoBadge.style.color = 'white';
    estadoBadge.style.padding = '0.5rem 1rem';
    estadoBadge.style.borderRadius = '20px';
}

// Cargar modal de eliminar
function cargarModalEliminar(boton) {
    var categoriaId = boton.getAttribute('data-id');

    document.getElementById('eliminar-id').value = categoriaId;
    document.getElementById('eliminar-id-display').textContent = categoriaId;
    document.getElementById('eliminar-nombre').textContent = boton.getAttribute('data-nombre');

    // Estado
    var estado = boton.getAttribute('data-estado');
    var estadoBadge = document.getElementById('eliminar-estado-badge');
    estadoBadge.textContent = estado;
    estadoBadge.className = estado === 'Activo' ? 'badge bg-success' : 'badge bg-danger';
    estadoBadge.style.background = estado === 'Activo' ? '#27ae60' : '#e74c3c';
    estadoBadge.style.color = 'white';
    estadoBadge.style.padding = '0.5rem 1rem';
    estadoBadge.style.borderRadius = '20px';

    var form = document.getElementById('formEliminarCategoria');
    form.action = '/Categoria/EliminarCategoria';
}

// Inicializar búsqueda
function inicializarBusqueda() {
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
}

// Mostrar página específica
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

// Actualizar estado de botones de paginación
function actualizarEstadoBotones(pagina, fin) {
    var btnAnterior = document.getElementById('btnAnterior');
    var btnSiguiente = document.getElementById('btnSiguiente');

    // Botón Anterior
    btnAnterior.disabled = pagina === 1;
    btnAnterior.style.opacity = pagina === 1 ? '0.5' : '1';
    btnAnterior.style.cursor = pagina === 1 ? 'not-allowed' : 'pointer';

    // Botón Siguiente
    btnSiguiente.disabled = fin >= todasLasFilas.length;
    btnSiguiente.style.opacity = fin >= todasLasFilas.length ? '0.5' : '1';
    btnSiguiente.style.cursor = fin >= todasLasFilas.length ? 'not-allowed' : 'pointer';
}

// Página anterior
function paginaAnterior() {
    if (paginaActual > 1) {
        paginaActual--;
        mostrarPagina(paginaActual);
    }
}

// Página siguiente
function paginaSiguiente() {
    var totalPaginas = Math.ceil(todasLasFilas.length / registrosPorPagina);
    if (paginaActual < totalPaginas) {
        paginaActual++;
        mostrarPagina(paginaActual);
    }
}

// ========================================
// FORMULARIO DE CATEGORÍA (CREAR/EDITAR)
// ========================================
function inicializarFormularioCategoria() {
    deshabilitarValidacionArchivo();
    mejorarExperienciaInputs();
    inicializarPreviewImagen();
}

// Deshabilitar validación de jQuery para archivo en edición
function deshabilitarValidacionArchivo() {
    var archivoInput = document.getElementById('archivoImagen');
    if (archivoInput && document.querySelector('input[name="IdCategoria"]')) {
        archivoInput.removeAttribute('data-val');
        archivoInput.removeAttribute('data-val-required');
    }
}

// Mejorar experiencia con inputs
function mejorarExperienciaInputs() {
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
}

// Inicializar preview de imagen
function inicializarPreviewImagen() {
    var inputArchivo = document.getElementById('archivoImagen');
    var preview = document.getElementById('preview');
    var fileName = document.getElementById('fileName');
    var dropZone = document.getElementById('dropZone');

    if (!inputArchivo || !preview || !fileName || !dropZone) return;

    inputArchivo.addEventListener('change', function (e) {
        var file = e.target.files[0];
        if (file) {
            mostrarPreviewImagen(file, preview, fileName, dropZone);
        }
    });

    // Drag and drop (opcional)
    configurarDragAndDrop(dropZone, inputArchivo, preview, fileName);
}

// Mostrar preview de imagen
function mostrarPreviewImagen(file, preview, fileName, dropZone) {
    // Validar tamaño (5MB)
    if (file.size > 5 * 1024 * 1024) {
        alert('El archivo es demasiado grande. El tamaño máximo es 5MB.');
        return;
    }

    // Validar tipo
    if (!file.type.match('image.*')) {
        alert('Por favor, seleccione un archivo de imagen válido.');
        return;
    }

    // Mostrar nombre
    fileName.textContent = file.name;

    // Mostrar preview
    var reader = new FileReader();
    reader.onload = function (e) {
        preview.innerHTML = '<img src="' + e.target.result + '" style="max-width: 300px; max-height: 300px; border-radius: 10px; box-shadow: var(--shadow); margin-top: 1rem;" />';
        dropZone.style.borderColor = 'var(--primary-color)';
        dropZone.style.background = 'var(--light-color)';
    };
    reader.readAsDataURL(file);
}

// Configurar drag and drop
function configurarDragAndDrop(dropZone, inputArchivo, preview, fileName) {
    dropZone.addEventListener('dragover', function (e) {
        e.preventDefault();
        this.style.borderColor = 'var(--primary-color)';
        this.style.background = 'rgba(212, 130, 92, 0.05)';
    });

    dropZone.addEventListener('dragleave', function (e) {
        e.preventDefault();
        this.style.borderColor = 'var(--secondary-color)';
        this.style.background = 'var(--light-color)';
    });

    dropZone.addEventListener('drop', function (e) {
        e.preventDefault();
        this.style.borderColor = 'var(--secondary-color)';
        this.style.background = 'var(--light-color)';

        var files = e.dataTransfer.files;
        if (files.length > 0) {
            inputArchivo.files = files;
            mostrarPreviewImagen(files[0], preview, fileName, dropZone);
        }
    });
}