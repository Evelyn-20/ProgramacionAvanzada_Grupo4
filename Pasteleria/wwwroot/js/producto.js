// ============================================
// PRODUCTO.JS - Funcionalidad de Productos
// ============================================

document.addEventListener('DOMContentLoaded', function () {

    // ============================================
    // PAGINACIÓN (solo para listado)
    // ============================================
    var paginaActual = 1;
    var registrosPorPagina = 5;
    var todasLasFilas = [];

    var table = document.getElementById('laTablaDeProductos');
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
            var productoId = this.getAttribute('data-id');
            var editUrl = this.getAttribute('data-edit-url') || '/Producto/EditarProducto';
            window.location.href = editUrl + '?id=' + productoId;
        });
    });

    // ============================================
    // BOTONES DE VER DETALLES
    // ============================================
    var botonesDetalles = document.querySelectorAll('.btn-detalles');
    botonesDetalles.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var productoId = this.getAttribute('data-id');

            // Cargar información básica
            var detallesId = document.getElementById('detalles-id');
            var detallesNombre = document.getElementById('detalles-nombre');
            var detallesCategoria = document.getElementById('detalles-categoria');
            var detallesDescripcion = document.getElementById('detalles-descripcion');
            var detallesCantidad = document.getElementById('detalles-cantidad');
            var detallesPrecio = document.getElementById('detalles-precio');
            var detallesImpuesto = document.getElementById('detalles-impuesto');
            var detallesFechaCreacion = document.getElementById('detalles-fecha-creacion');
            var detallesFechaActualizacion = document.getElementById('detalles-fecha-actualizacion');
            var estadoBadge = document.getElementById('detalles-estado-badge');
            var imagenElement = document.getElementById('detalles-imagen');

            if (detallesId) detallesId.textContent = productoId;
            if (detallesNombre) detallesNombre.textContent = this.getAttribute('data-nombre');
            if (detallesCategoria) detallesCategoria.textContent = this.getAttribute('data-categoria');
            if (detallesDescripcion) detallesDescripcion.textContent = this.getAttribute('data-descripcion');
            if (detallesCantidad) detallesCantidad.textContent = this.getAttribute('data-cantidad');
            if (detallesPrecio) detallesPrecio.textContent = '₡' + this.getAttribute('data-precio');
            if (detallesImpuesto) detallesImpuesto.textContent = this.getAttribute('data-impuesto') + '%';
            if (detallesFechaCreacion) detallesFechaCreacion.textContent = this.getAttribute('data-fecha-creacion');
            if (detallesFechaActualizacion) detallesFechaActualizacion.textContent = this.getAttribute('data-fecha-actualizacion');

            // Cargar imagen
            if (imagenElement) {
                var imageUrl = this.getAttribute('data-image-url') || '/Producto/ObtenerImagenProducto?id=' + productoId;
                imagenElement.src = imageUrl;
                imagenElement.alt = this.getAttribute('data-nombre');
            }

            // Actualizar badge de estado
            if (estadoBadge) {
                var estado = this.getAttribute('data-estado');
                estadoBadge.textContent = estado;
                estadoBadge.style.padding = '0.5rem 1rem';
                estadoBadge.style.borderRadius = '20px';
                estadoBadge.style.display = 'inline-block';
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
            var eliminarId = document.getElementById('eliminar-id');
            var eliminarIdDisplay = document.getElementById('eliminar-id-display');
            var eliminarNombre = document.getElementById('eliminar-nombre');
            var eliminarCategoria = document.getElementById('eliminar-categoria');
            var eliminarDescripcion = document.getElementById('eliminar-descripcion');
            var eliminarCantidad = document.getElementById('eliminar-cantidad');
            var eliminarPrecio = document.getElementById('eliminar-precio');
            var eliminarEstadoBadge = document.getElementById('eliminar-estado-badge');

            if (eliminarId) eliminarId.value = this.getAttribute('data-id');
            if (eliminarIdDisplay) eliminarIdDisplay.textContent = this.getAttribute('data-id');
            if (eliminarNombre) eliminarNombre.textContent = this.getAttribute('data-nombre');
            if (eliminarCategoria) eliminarCategoria.textContent = this.getAttribute('data-categoria');
            if (eliminarDescripcion) eliminarDescripcion.textContent = this.getAttribute('data-descripcion');
            if (eliminarCantidad) eliminarCantidad.textContent = this.getAttribute('data-cantidad');
            if (eliminarPrecio) eliminarPrecio.textContent = '₡' + this.getAttribute('data-precio');

            if (eliminarEstadoBadge) {
                var estado = this.getAttribute('data-estado');
                eliminarEstadoBadge.textContent = estado;
                eliminarEstadoBadge.style.padding = '0.5rem 1rem';
                eliminarEstadoBadge.style.borderRadius = '20px';
                eliminarEstadoBadge.style.fontWeight = '600';
                eliminarEstadoBadge.style.color = 'white';

                if (estado === 'Activo') {
                    eliminarEstadoBadge.className = 'badge bg-success';
                    eliminarEstadoBadge.style.background = '#27ae60';
                } else {
                    eliminarEstadoBadge.className = 'badge bg-danger';
                    eliminarEstadoBadge.style.background = '#e74c3c';
                }
            }

            var form = document.getElementById('formEliminarProducto');
            var deleteUrl = btn.getAttribute('data-delete-url') || '/Producto/EliminarProducto';
            if (form) form.action = deleteUrl;
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
    // PREVIEW DE IMAGEN
    // ============================================
    var inputArchivo = document.getElementById('archivoImagen');
    var preview = document.getElementById('preview');
    var fileName = document.getElementById('fileName');
    var dropZone = document.getElementById('dropZone');

    if (inputArchivo) {
        inputArchivo.addEventListener('change', function (e) {
            var file = e.target.files[0];
            if (file) {
                // Validar tamaño (5MB máximo)
                var maxSize = 5 * 1024 * 1024; // 5MB en bytes
                if (file.size > maxSize) {
                    alert('El archivo es demasiado grande. El tamaño máximo es 5MB.');
                    this.value = '';
                    return;
                }

                // Validar tipo de archivo
                var validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/bmp'];
                if (!validTypes.includes(file.type)) {
                    alert('Tipo de archivo no válido. Solo se permiten: JPG, JPEG, PNG, GIF, BMP');
                    this.value = '';
                    return;
                }

                // Mostrar nombre del archivo
                if (fileName) {
                    fileName.textContent = '📁 ' + file.name;
                    fileName.style.color = 'var(--primary-color)';
                    fileName.style.fontWeight = '600';
                }

                // Preview de imagen
                if (preview && file.type.match('image.*')) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        preview.innerHTML = '<img src="' + e.target.result + '" style="max-width: 300px; max-height: 300px; border-radius: 10px; box-shadow: var(--shadow); margin-top: 1rem; object-fit: cover;" />';

                        if (dropZone) {
                            dropZone.style.borderColor = 'var(--primary-color)';
                            dropZone.style.background = 'rgba(212, 130, 92, 0.05)';
                        }
                    };
                    reader.readAsDataURL(file);
                }
            } else {
                // Limpiar preview si no hay archivo
                if (preview) preview.innerHTML = '';
                if (fileName) fileName.textContent = '';
                if (dropZone) {
                    dropZone.style.borderColor = 'var(--secondary-color)';
                    dropZone.style.background = 'var(--light-color)';
                }
            }
        });

        // Drag and Drop (opcional)
        if (dropZone) {
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

                if (e.dataTransfer.files.length > 0) {
                    inputArchivo.files = e.dataTransfer.files;
                    // Disparar el evento change manualmente
                    var event = new Event('change', { bubbles: true });
                    inputArchivo.dispatchEvent(event);
                }
            });
        }
    }

    // ============================================
    // DESHABILITAR VALIDACIÓN DE ARCHIVO EN EDICIÓN
    // ============================================
    var form = document.querySelector('form');
    if (form && inputArchivo) {
        // Si estamos en la página de edición, remover validación requerida del archivo
        var esEdicion = window.location.href.indexOf('Editar') > -1;
        if (esEdicion) {
            inputArchivo.removeAttribute('data-val');
            inputArchivo.removeAttribute('data-val-required');
            inputArchivo.removeAttribute('required');
        }
    }

    // ============================================
    // HOVER EN FILAS DE TABLA
    // ============================================
    var filasTabla = document.querySelectorAll('#laTablaDeProductos tbody tr');
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
// FUNCIÓN AUXILIAR: FORMATEAR PRECIO
// ============================================
function formatearPrecio(precio) {
    return parseFloat(precio).toLocaleString('es-CR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

// ============================================
// FUNCIÓN AUXILIAR: VALIDAR IMAGEN
// ============================================
function validarImagen(file) {
    var maxSize = 5 * 1024 * 1024; // 5MB
    var validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/bmp'];

    if (file.size > maxSize) {
        return { valido: false, mensaje: 'El archivo es demasiado grande. Tamaño máximo: 5MB' };
    }

    if (!validTypes.includes(file.type)) {
        return { valido: false, mensaje: 'Tipo de archivo no válido. Solo JPG, JPEG, PNG, GIF, BMP' };
    }

    return { valido: true };
}