document.addEventListener('DOMContentLoaded', function () {

    // Paginacion
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

    // Boton de detalles (Administrador/Empleado)
    var botonesDetalles = document.querySelectorAll('.btn-detalles');
    botonesDetalles.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var pedidoId = this.getAttribute('data-id');
            cargarDetallesPedido(pedidoId);
        });
    });

    // Boton de detalles (Cliente)
    var botonesDetallesCliente = document.querySelectorAll('.btn-detalles-cliente');
    botonesDetallesCliente.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var pedidoId = this.getAttribute('data-id');
            cargarDetallesPedidoCliente(pedidoId);
        });
    });

    // Función para cargar detalles del pedido via AJAX (para clientes)
    function cargarDetallesPedidoCliente(pedidoId) {
        // Mostrar loading en el modal de cliente
        var productosContainer = document.getElementById('cliente-pedido-productos');
        if (productosContainer) {
            productosContainer.innerHTML = '<div style="text-align: center; padding: 2rem;"><i class="fas fa-spinner fa-spin" style="font-size: 2rem; color: var(--primary-color);"></i><p style="margin-top: 1rem;">Cargando tus productos...</p></div>';
        }

        fetch('/Pedido/ObtenerDetalles/' + pedidoId)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Error al cargar detalles');
                }
                return response.json();
            })
            .then(data => {
                actualizarModalDetallesCliente(data);
            })
            .catch(error => {
                console.error('Error:', error);
                if (productosContainer) {
                    productosContainer.innerHTML = '<div style="text-align: center; padding: 2rem; color: #e74c3c;"><i class="fas fa-exclamation-circle" style="font-size: 2rem;"></i><p style="margin-top: 1rem;">Error al cargar los productos</p></div>';
                }
            });
    }

    // Función para actualizar el modal de cliente con los datos del pedido
    function actualizarModalDetallesCliente(data) {
        var pedido = data.pedido;
        var productos = data.productos;

        // Actualizar información básica
        var clientePedidoId = document.getElementById('cliente-pedido-id');
        var clientePedidoFecha = document.getElementById('cliente-pedido-fecha');
        var clientePedidoSubtotal = document.getElementById('cliente-pedido-subtotal');
        var clientePedidoDescuento = document.getElementById('cliente-pedido-descuento');
        var clientePedidoDescuentoRow = document.getElementById('cliente-pedido-descuento-row');
        var clientePedidoImpuesto = document.getElementById('cliente-pedido-impuesto');
        var clientePedidoTotal = document.getElementById('cliente-pedido-total');
        var clientePedidoTotalHeader = document.getElementById('cliente-pedido-total-header');
        var estadoBadge = document.getElementById('cliente-pedido-estado-badge');

        if (clientePedidoId) clientePedidoId.textContent = pedido.id;
        if (clientePedidoFecha) clientePedidoFecha.textContent = pedido.fecha;

        // Formatear y mostrar totales
        if (clientePedidoSubtotal) {
            clientePedidoSubtotal.textContent = '₡' + formatearNumero(pedido.subtotal);
        }

        if (clientePedidoImpuesto) {
            clientePedidoImpuesto.textContent = '₡' + formatearNumero(pedido.impuesto);
        }

        if (clientePedidoTotal) {
            clientePedidoTotal.textContent = '₡' + formatearNumero(pedido.total);
        }

        if (clientePedidoTotalHeader) {
            clientePedidoTotalHeader.textContent = '₡' + formatearNumero(pedido.total);
        }

        // Mostrar/ocultar descuento
        if (pedido.descuento && pedido.descuento > 0) {
            if (clientePedidoDescuento) {
                clientePedidoDescuento.textContent = '-₡' + formatearNumero(pedido.descuento);
            }
            if (clientePedidoDescuentoRow) {
                clientePedidoDescuentoRow.style.display = 'flex';
            }
        } else {
            if (clientePedidoDescuentoRow) {
                clientePedidoDescuentoRow.style.display = 'none';
            }
        }

        // Actualizar badge de estado
        if (estadoBadge) {
            var icono = obtenerIconoEstado(pedido.estado);
            estadoBadge.innerHTML = '<i class="fas ' + icono + '"></i> ' + pedido.estado;
            var estadoColor = obtenerColorEstado(pedido.estado);
            estadoBadge.style.background = estadoColor;
            estadoBadge.style.color = 'white';
        }

        // Actualizar lista de productos
        var productosContainer = document.getElementById('cliente-pedido-productos');
        if (productosContainer) {
            if (productos && productos.length > 0) {
                var productosHTML = productos.map(function (producto, index) {
                    var subtotalConDescuento = producto.subtotal - producto.descuento;
                    return `
                        <div style="background: var(--white); padding: 1.25rem; border-radius: 8px; margin-bottom: 0.75rem; box-shadow: 0 2px 4px rgba(0,0,0,0.05); transition: transform 0.2s;" onmouseover="this.style.transform='translateX(5px)'" onmouseout="this.style.transform='translateX(0)'">
                            <div style="display: flex; justify-content: space-between; align-items: start; gap: 1rem;">
                                <div style="flex: 1;">
                                    <div style="display: flex; align-items: center; gap: 0.5rem; margin-bottom: 0.5rem;">
                                        <span style="background: var(--primary-color); color: var(--white); width: 24px; height: 24px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 0.75rem; font-weight: 600;">${index + 1}</span>
                                        <strong style="color: var(--dark-color); font-size: 1.1rem;">${producto.nombre}</strong>
                                    </div>
                                    <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(120px, 1fr)); gap: 0.75rem; margin-top: 0.75rem;">
                                        <div style="background: var(--light-color); padding: 0.5rem; border-radius: 5px;">
                                            <div style="font-size: 0.75rem; color: var(--text-color); margin-bottom: 0.25rem;">Precio unitario</div>
                                            <div style="font-weight: 600; color: var(--dark-color);">₡${formatearNumero(producto.precio)}</div>
                                        </div>
                                        <div style="background: var(--light-color); padding: 0.5rem; border-radius: 5px;">
                                            <div style="font-size: 0.75rem; color: var(--text-color); margin-bottom: 0.25rem;">Cantidad</div>
                                            <div style="font-weight: 600; color: var(--dark-color);">${producto.cantidad} ${producto.cantidad === 1 ? 'unidad' : 'unidades'}</div>
                                        </div>
                                        ${producto.descuento > 0 ? `
                                        <div style="background: #e8f5e9; padding: 0.5rem; border-radius: 5px;">
                                            <div style="font-size: 0.75rem; color: #2e7d32; margin-bottom: 0.25rem;">Descuento</div>
                                            <div style="font-weight: 600; color: #27ae60;">-₡${formatearNumero(producto.descuento)}</div>
                                        </div>
                                        ` : ''}
                                    </div>
                                </div>
                                <div style="text-align: right; min-width: 100px;">
                                    <div style="font-size: 0.75rem; color: var(--text-color); margin-bottom: 0.25rem;">Subtotal</div>
                                    <div style="color: var(--primary-color); font-weight: 700; font-size: 1.3rem;">₡${formatearNumero(subtotalConDescuento)}</div>
                                </div>
                            </div>
                        </div>
                    `;
                }).join('');
                productosContainer.innerHTML = productosHTML;
            } else {
                productosContainer.innerHTML = '<div style="text-align: center; padding: 2rem; color: var(--text-color);">No hay productos en este pedido</div>';
            }
        }
    }

    // Función para cargar detalles del pedido via AJAX
    function cargarDetallesPedido(pedidoId) {
        // Mostrar loading en el modal
        var productosContainer = document.getElementById('detalles-pedido-productos');
        if (productosContainer) {
            productosContainer.innerHTML = '<div style="text-align: center; padding: 2rem;"><i class="fas fa-spinner fa-spin" style="font-size: 2rem; color: var(--primary-color);"></i><p style="margin-top: 1rem;">Cargando productos...</p></div>';
        }

        fetch('/Pedido/ObtenerDetalles/' + pedidoId)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Error al cargar detalles');
                }
                return response.json();
            })
            .then(data => {
                actualizarModalDetalles(data);
            })
            .catch(error => {
                console.error('Error:', error);
                if (productosContainer) {
                    productosContainer.innerHTML = '<div style="text-align: center; padding: 2rem; color: #e74c3c;"><i class="fas fa-exclamation-circle" style="font-size: 2rem;"></i><p style="margin-top: 1rem;">Error al cargar los productos</p></div>';
                }
            });
    }

    // Función para actualizar el modal con los datos del pedido
    function actualizarModalDetalles(data) {
        var pedido = data.pedido;
        var productos = data.productos;

        // Actualizar información básica
        var detallesPedidoId = document.getElementById('detalles-pedido-id');
        var detallesPedidoCliente = document.getElementById('detalles-pedido-cliente');
        var detallesPedidoFecha = document.getElementById('detalles-pedido-fecha');
        var detallesPedidoSubtotal = document.getElementById('detalles-pedido-subtotal');
        var detallesPedidoDescuento = document.getElementById('detalles-pedido-descuento');
        var detallesPedidoDescuentoRow = document.getElementById('detalles-pedido-descuento-row');
        var detallesPedidoImpuesto = document.getElementById('detalles-pedido-impuesto');
        var detallesPedidoTotal = document.getElementById('detalles-pedido-total');
        var estadoBadge = document.getElementById('detalles-pedido-estado-badge');

        if (detallesPedidoId) detallesPedidoId.textContent = pedido.id;
        if (detallesPedidoCliente) detallesPedidoCliente.textContent = pedido.cliente;
        if (detallesPedidoFecha) detallesPedidoFecha.textContent = pedido.fecha;

        // Formatear y mostrar totales
        if (detallesPedidoSubtotal) {
            detallesPedidoSubtotal.textContent = '₡' + formatearNumero(pedido.subtotal);
        }

        if (detallesPedidoImpuesto) {
            detallesPedidoImpuesto.textContent = '₡' + formatearNumero(pedido.impuesto);
        }

        if (detallesPedidoTotal) {
            detallesPedidoTotal.textContent = '₡' + formatearNumero(pedido.total);
        }

        // Mostrar/ocultar descuento
        if (pedido.descuento && pedido.descuento > 0) {
            if (detallesPedidoDescuento) {
                detallesPedidoDescuento.textContent = '-₡' + formatearNumero(pedido.descuento);
            }
            if (detallesPedidoDescuentoRow) {
                detallesPedidoDescuentoRow.style.display = 'flex';
            }
        } else {
            if (detallesPedidoDescuentoRow) {
                detallesPedidoDescuentoRow.style.display = 'none';
            }
        }

        // Actualizar badge de estado
        if (estadoBadge) {
            var icono = obtenerIconoEstado(pedido.estado);
            estadoBadge.innerHTML = '<i class="fas ' + icono + '"></i> ' + pedido.estado;
            var estadoColor = obtenerColorEstado(pedido.estado);
            estadoBadge.style.background = estadoColor;
            estadoBadge.style.color = 'white';
            estadoBadge.style.padding = '0.5rem 1rem';
            estadoBadge.style.borderRadius = '20px';
            estadoBadge.style.display = 'inline-block';
            estadoBadge.style.fontWeight = '600';
        }

        // Actualizar lista de productos
        var productosContainer = document.getElementById('detalles-pedido-productos');
        if (productosContainer) {
            if (productos && productos.length > 0) {
                var productosHTML = productos.map(function (producto) {
                    var subtotalConDescuento = producto.subtotal - producto.descuento;
                    return `
                        <div style="padding: 1rem; border-bottom: 1px solid var(--secondary-color); display: flex; justify-content: space-between; align-items: center;">
                            <div style="flex: 1;">
                                <strong style="color: var(--dark-color); display: block; margin-bottom: 0.25rem;">${producto.nombre}</strong>
                                <div style="display: flex; gap: 1rem; font-size: 0.9rem; color: var(--text-color);">
                                    <span><i class="fas fa-tag"></i> ₡${formatearNumero(producto.precio)}</span>
                                    <span><i class="fas fa-times"></i> ${producto.cantidad}</span>
                                    ${producto.descuento > 0 ? `<span style="color: #27ae60;"><i class="fas fa-percent"></i> -₡${formatearNumero(producto.descuento)}</span>` : ''}
                                </div>
                            </div>
                            <div style="text-align: right;">
                                <strong style="color: var(--primary-color); font-size: 1.1rem;">₡${formatearNumero(subtotalConDescuento)}</strong>
                            </div>
                        </div>
                    `;
                }).join('');
                productosContainer.innerHTML = productosHTML;
            } else {
                productosContainer.innerHTML = '<div style="text-align: center; padding: 2rem; color: var(--text-color);">No hay productos en este pedido</div>';
            }
        }
    }

    // Boton de editar estado
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

    // Busqueda
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

    // Select del estado
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

    // Hover sobre las filas de la tabla
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

// Formatear los numeros
function formatearNumero(numero) {
    if (!numero) return '0.00';
    return parseFloat(numero).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

// Obtener color segun el estado
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

// Obtener Icon segun el estado
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