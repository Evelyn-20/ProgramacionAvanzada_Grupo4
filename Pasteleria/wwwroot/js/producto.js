document.addEventListener('DOMContentLoaded', function () {

    var paginaActual = 1;
    var registrosPorPagina = 10;
    var todasLasFilas = [];
    var table = document.getElementById('laTablaDeProductos');

    if (table && table.getElementsByTagName('tbody')[0]) {
        todasLasFilas = Array.from(table.getElementsByTagName('tbody')[0].getElementsByTagName('tr'));
        if (todasLasFilas.length > 0 && !todasLasFilas[0].querySelector('td[colspan]')) {
            mostrarPagina(paginaActual);
        }
    }

    // Modal auto-closed
    function mostrarNotificacion(mensaje, tipo) {
        const iconos = {
            'success': 'fa-check-circle',
            'error': 'fa-exclamation-triangle',
            'warning': 'fa-exclamation-circle',
            'info': 'fa-info-circle'
        };

        const colores = {
            'success': '#27ae60',
            'error': '#e74c3c',
            'warning': '#f39c12',
            'info': '#3498db'
        };

        const modalHtml = `
            <div class="modal fade" id="modalNotificacion" tabindex="-1" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content" style="border: none; border-radius: 15px; overflow: hidden; box-shadow: 0 10px 40px rgba(0,0,0,0.2);">
                        <div class="modal-body" style="padding: 2rem; text-align: center; background: var(--white);">
                            <div style="width: 80px; height: 80px; margin: 0 auto 1.5rem; background: ${colores[tipo]}; border-radius: 50%; display: flex; align-items: center; justify-content: center; animation: scaleIn 0.3s ease-out;">
                                <i class="fas ${iconos[tipo]}" style="font-size: 2.5rem; color: white;"></i>
                            </div>
                            <h5 style="color: var(--dark-color); margin-bottom: 1rem; font-weight: 600;">${mensaje}</h5>
                            <div style="width: 100%; height: 4px; background: #e0e0e0; border-radius: 2px; overflow: hidden; margin-top: 1.5rem;">
                                <div id="barraProgreso" style="width: 100%; height: 100%; background: ${colores[tipo]}; transition: width 3s linear;"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;

        if (!document.getElementById('notificacion-styles')) {
            const styles = document.createElement('style');
            styles.id = 'notificacion-styles';
            styles.textContent = `
                @keyframes scaleIn {
                    from {
                        transform: scale(0);
                        opacity: 0;
                    }
                    to {
                        transform: scale(1);
                        opacity: 1;
                    }
                }
            `;
            document.head.appendChild(styles);
        }

        const modalAnterior = document.getElementById('modalNotificacion');
        if (modalAnterior) {
            modalAnterior.remove();
        }

        document.body.insertAdjacentHTML('beforeend', modalHtml);

        const modalElement = document.getElementById('modalNotificacion');
        const modal = new bootstrap.Modal(modalElement);
        modal.show();

        setTimeout(function () {
            const barra = document.getElementById('barraProgreso');
            if (barra) {
                barra.style.width = '0%';
            }
        }, 100);

        setTimeout(function () {
            modal.hide();
            setTimeout(function () {
                modalElement.remove();
            }, 300);
        }, 3000);
    }

    // Verificar mensajes de TempData
    const successMessage = document.querySelector('[data-success-message]');
    const errorMessage = document.querySelector('[data-error-message]');

    if (successMessage) {
        const mensaje = successMessage.getAttribute('data-success-message');
        mostrarNotificacion(mensaje, 'success');
    }

    if (errorMessage) {
        const mensaje = errorMessage.getAttribute('data-error-message');
        mostrarNotificacion(mensaje, 'error');
    }

    // Paginacion
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

    // Busqueda
    const inputBuscar = document.getElementById('buscar');

    function buscarEnTabla(texto) {
        texto = texto.toLowerCase().trim();

        if (!texto) {
            todasLasFilas.forEach(function (fila) {
                if (!fila.querySelector('td[colspan]')) {
                    fila.style.display = '';
                }
            });
            actualizarContadores();
            paginaActual = 1;
            mostrarPagina(paginaActual);
            return;
        }

        let filasVisibles = 0;
        todasLasFilas.forEach(function (fila) {
            if (fila.querySelector('td[colspan]')) {
                return;
            }

            const celdas = fila.getElementsByTagName('td');
            let coincide = false;

            const nombre = celdas[2] ? celdas[2].textContent.toLowerCase() : '';
            if (nombre.includes(texto)) {
                coincide = true;
            }

            const categoria = celdas[3] ? celdas[3].textContent.toLowerCase() : '';
            if (categoria.includes(texto)) {
                coincide = true;
            }

            const descripcion = celdas[4] ? celdas[4].textContent.toLowerCase() : '';
            if (descripcion.includes(texto)) {
                coincide = true;
            }

            const cantidad = celdas[5] ? celdas[5].textContent.toLowerCase() : '';
            if (cantidad.includes(texto)) {
                coincide = true;
            }

            const precio = celdas[6] ? celdas[6].textContent.toLowerCase().replace('₡', '').replace(',', '') : '';
            if (precio.includes(texto)) {
                coincide = true;
            }

            const id = celdas[0] ? celdas[0].textContent.toLowerCase() : '';
            if (id.includes(texto)) {
                coincide = true;
            }

            if (coincide) {
                fila.style.display = '';
                filasVisibles++;
            } else {
                fila.style.display = 'none';
            }
        });

        actualizarContadores();

        const tbody = table.getElementsByTagName('tbody')[0];
        let filaNoResultados = tbody.querySelector('.fila-no-resultados');

        if (filasVisibles === 0) {
            if (!filaNoResultados) {
                filaNoResultados = document.createElement('tr');
                filaNoResultados.className = 'fila-no-resultados';
                filaNoResultados.innerHTML = `
                    <td colspan="10" style="padding: 3rem; text-align: center; color: var(--text-color);">
                        <i class="fas fa-search" style="font-size: 3rem; color: var(--secondary-color); opacity: 0.5; display: block; margin-bottom: 1rem;"></i>
                        <p style="font-size: 1.2rem; margin: 0;">No se encontraron resultados para "<strong>${texto}</strong>"</p>
                        <p style="margin-top: 0.5rem; color: var(--text-color); opacity: 0.7;">Intenta con otro término de búsqueda</p>
                    </td>
                `;
                tbody.appendChild(filaNoResultados);
            }
        } else {
            if (filaNoResultados) {
                filaNoResultados.remove();
            }
        }
    }

    function actualizarContadores() {
        const filasVisibles = todasLasFilas.filter(function (fila) {
            return !fila.querySelector('td[colspan]') && fila.style.display !== 'none';
        });

        const totalVisibles = filasVisibles.length;
        const startRecord = document.getElementById('startRecord');
        const endRecord = document.getElementById('endRecord');
        const totalRecordsEl = document.getElementById('totalRecords');

        if (startRecord) startRecord.textContent = totalVisibles > 0 ? '1' : '0';
        if (endRecord) endRecord.textContent = totalVisibles;
        if (totalRecordsEl) totalRecordsEl.textContent = totalVisibles;

        const btnAnterior = document.getElementById('btnAnterior');
        const btnSiguiente = document.getElementById('btnSiguiente');
        const textoBusqueda = inputBuscar ? inputBuscar.value.trim() : '';

        if (textoBusqueda) {
            if (btnAnterior) btnAnterior.style.display = 'none';
            if (btnSiguiente) btnSiguiente.style.display = 'none';
        } else {
            if (btnAnterior) btnAnterior.style.display = '';
            if (btnSiguiente) btnSiguiente.style.display = '';
            paginaActual = 1;
            mostrarPagina(paginaActual);
        }
    }

    if (inputBuscar) {
        inputBuscar.addEventListener('input', function () {
            buscarEnTabla(this.value);
        });

        inputBuscar.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                buscarEnTabla(this.value);
            }
        });
    }

    // Botones
    var botonesEditar = document.querySelectorAll('.btn-editar');
    botonesEditar.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var productoId = this.getAttribute('data-id');
            var editUrl = this.getAttribute('data-edit-url') || '/Producto/EditarProducto';
            window.location.href = editUrl + '?id=' + productoId;
        });
    });

    var botonesDetalles = document.querySelectorAll('.btn-detalles');
    botonesDetalles.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var productoId = this.getAttribute('data-id');

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

            if (imagenElement) {
                var imageUrl = this.getAttribute('data-image-url') || '/Producto/ObtenerImagenProducto?id=' + productoId;
                imagenElement.src = imageUrl;
                imagenElement.alt = this.getAttribute('data-nombre');
            }

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

    // Efectos
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

    // Preview de imagen
    var inputArchivo = document.getElementById('archivoImagen');
    var preview = document.getElementById('preview');
    var fileName = document.getElementById('fileName');
    var dropZone = document.getElementById('dropZone');

    if (inputArchivo) {
        inputArchivo.addEventListener('change', function (e) {
            var file = e.target.files[0];
            if (file) {
                var maxSize = 5 * 1024 * 1024;
                if (file.size > maxSize) {
                    mostrarNotificacion('El archivo es demasiado grande. El tamaño máximo es 5MB.', 'error');
                    this.value = '';
                    return;
                }

                var validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/bmp'];
                if (!validTypes.includes(file.type)) {
                    mostrarNotificacion('Tipo de archivo no válido. Solo se permiten: JPG, JPEG, PNG, GIF, BMP', 'error');
                    this.value = '';
                    return;
                }

                if (fileName) {
                    fileName.textContent = '📁 ' + file.name;
                    fileName.style.color = 'var(--primary-color)';
                    fileName.style.fontWeight = '600';
                }

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
                if (preview) preview.innerHTML = '';
                if (fileName) fileName.textContent = '';
                if (dropZone) {
                    dropZone.style.borderColor = 'var(--secondary-color)';
                    dropZone.style.background = 'var(--light-color)';
                }
            }
        });

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
                    var event = new Event('change', { bubbles: true });
                    inputArchivo.dispatchEvent(event);
                }
            });
        }
    }

    var form = document.querySelector('form');
    if (form && inputArchivo) {
        var esEdicion = window.location.href.indexOf('Editar') > -1;
        if (esEdicion) {
            inputArchivo.removeAttribute('data-val');
            inputArchivo.removeAttribute('data-val-required');
            inputArchivo.removeAttribute('required');
        }
    }

    var filasTabla = document.querySelectorAll('#laTablaDeProductos tbody tr');
    filasTabla.forEach(function (fila) {
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

function formatearPrecio(precio) {
    return parseFloat(precio).toLocaleString('es-CR', {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    });
}

function validarImagen(file) {
    var maxSize = 5 * 1024 * 1024;
    var validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/bmp'];

    if (file.size > maxSize) {
        return { valido: false, mensaje: 'El archivo es demasiado grande. Tamaño máximo: 5MB' };
    }

    if (!validTypes.includes(file.type)) {
        return { valido: false, mensaje: 'Tipo de archivo no válido. Solo JPG, JPEG, PNG, GIF, BMP' };
    }

    return { valido: true };
}