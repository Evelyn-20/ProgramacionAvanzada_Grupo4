let carritoActual = [];
let tiempoDebounce = null;

document.addEventListener('DOMContentLoaded', function () {
    inicializarBuscadorProductos();
    inicializarCalculoEnVivo();
    actualizarContadorCarrito();
});

// Buscar productos
function inicializarBuscadorProductos() {
    const buscadorInput = document.getElementById('buscar-producto');

    if (!buscadorInput) return;

    buscadorInput.addEventListener('input', function (e) {
        const query = e.target.value.trim();

        // Debounce para no hacer muchas peticiones
        clearTimeout(tiempoDebounce);

        tiempoDebounce = setTimeout(function () {
            if (query.length >= 2) {
                buscarProductosAjax(query);
            } else {
                ocultarResultadosBusqueda();
            }
        }, 300);
    });

    // Cerrar resultados al hacer click fuera
    document.addEventListener('click', function (e) {
        if (!e.target.closest('#contenedor-busqueda')) {
            ocultarResultadosBusqueda();
        }
    });
}

// Realizar búsqueda AJAX de productos
function buscarProductosAjax(query) {
    fetch(`/api/pedidosapi/buscar-productos?q=${encodeURIComponent(query)}`)
        .then(response => {
            if (!response.ok) throw new Error('Error en la búsqueda');
            return response.json();
        })
        .then(data => {
            mostrarResultadosBusqueda(data);
        })
        .catch(error => {
            console.error('Error al buscar productos:', error);
            mostrarMensaje('Error al buscar productos', 'error');
        });
}

// Mostrar resultados de búsqueda
function mostrarResultadosBusqueda(productos) {
    let contenedor = document.getElementById('resultados-busqueda');

    if (!contenedor) {
        contenedor = document.createElement('div');
        contenedor.id = 'resultados-busqueda';
        contenedor.style.cssText = `
            position: absolute;
            top: 100%;
            left: 0;
            right: 0;
            background: white;
            border: 2px solid var(--secondary-color);
            border-radius: 10px;
            max-height: 400px;
            overflow-y: auto;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            z-index: 1000;
            margin-top: 5px;
        `;
        document.getElementById('contenedor-busqueda').appendChild(contenedor);
    }

    if (productos.length === 0) {
        contenedor.innerHTML = '<div style="padding: 1rem; text-align: center; color: var(--text-color);">No se encontraron productos</div>';
        return;
    }

    contenedor.innerHTML = productos.map(p => `
        <div class="resultado-producto" 
             data-id="${p.id}"
             data-nombre="${p.nombre}"
             data-precio="${p.precio}"
             data-impuesto="${p.impuesto}"
             data-stock="${p.stock}"
             style="padding: 1rem; border-bottom: 1px solid var(--secondary-color); cursor: pointer; transition: background 0.3s;"
             onmouseover="this.style.background='var(--light-color)'"
             onmouseout="this.style.background='white'"
             onclick="seleccionarProducto(${p.id}, '${p.nombre}', ${p.precio}, ${p.impuesto}, ${p.stock})">
            <div style="display: flex; justify-content: space-between; align-items: center;">
                <div>
                    <strong style="color: var(--dark-color);">${p.nombre}</strong>
                    <p style="margin: 0.25rem 0 0 0; color: var(--text-color); font-size: 0.85rem;">${p.descripcion || ''}</p>
                </div>
                <div style="text-align: right;">
                    <div style="color: var(--primary-color); font-weight: 600; font-size: 1.1rem;">₡${p.precio.toFixed(2)}</div>
                    <div style="font-size: 0.8rem; color: var(--text-color);">Stock: ${p.stock}</div>
                </div>
            </div>
        </div>
    `).join('');
}

// Ocultar resultados de búsqueda
function ocultarResultadosBusqueda() {
    const contenedor = document.getElementById('resultados-busqueda');
    if (contenedor) {
        contenedor.remove();
    }
}

// Seleccionar producto y agregarlo al carrito temporal
function seleccionarProducto(id, nombre, precio, impuesto, stock) {
    ocultarResultadosBusqueda();

    // Verificar si ya está en el carrito
    const existente = carritoActual.find(item => item.productoId === id);

    if (existente) {
        if (existente.cantidad < stock) {
            existente.cantidad++;
            mostrarMensaje(`Cantidad de "${nombre}" actualizada`, 'success');
        } else {
            mostrarMensaje(`Stock máximo alcanzado (${stock} unidades)`, 'warning');
            return;
        }
    } else {
        carritoActual.push({
            productoId: id,
            nombre: nombre,
            precio: precio,
            impuesto: impuesto,
            stock: stock,
            cantidad: 1,
            descuento: 0
        });
        mostrarMensaje(`"${nombre}" agregado al carrito`, 'success');
    }

    actualizarVisualizacionCarrito();
    calcularTotalesEnVivo();

    // Limpiar búsqueda
    const buscadorInput = document.getElementById('buscar-producto');
    if (buscadorInput) buscadorInput.value = '';
}

// Calculo de los totales con el AJAX
function inicializarCalculoEnVivo() {
    // Escuchar cambios en cantidades y descuentos
    document.addEventListener('input', function (e) {
        if (e.target.matches('.input-cantidad, .input-descuento')) {
            calcularTotalesEnVivo();
        }
    });
}

function calcularTotalesEnVivo() {
    if (carritoActual.length === 0) {
        actualizarTotalesUI(0, 0, 0, 0);
        return;
    }

    // Preparar datos para enviar
    const items = carritoActual.map(item => ({
        productoId: item.productoId,
        cantidad: item.cantidad,
        descuento: item.descuento || 0
    }));

    // Realizar petición AJAX
    fetch('/api/pedidosapi/calcular-totales', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(items)
    })
        .then(response => {
            if (!response.ok) throw new Error('Error al calcular totales');
            return response.json();
        })
        .then(data => {
            actualizarTotalesUI(data.subtotal, data.descuento, data.impuesto, data.total);
        })
        .catch(error => {
            console.error('Error al calcular totales:', error);
            mostrarMensaje('Error al calcular totales', 'error');
        });
}

// Actualizar UI con los totales calculados
function actualizarTotalesUI(subtotal, descuento, impuesto, total) {
    const elementos = {
        subtotal: document.getElementById('subtotal-pedido'),
        descuento: document.getElementById('descuento-pedido'),
        impuesto: document.getElementById('impuesto-pedido'),
        total: document.getElementById('total-pedido')
    };

    if (elementos.subtotal) elementos.subtotal.textContent = `₡${subtotal.toFixed(2)}`;
    if (elementos.descuento) elementos.descuento.textContent = `₡${descuento.toFixed(2)}`;
    if (elementos.impuesto) elementos.impuesto.textContent = `₡${impuesto.toFixed(2)}`;
    if (elementos.total) {
        elementos.total.textContent = `₡${total.toFixed(2)}`;
        elementos.total.style.animation = 'pulse 0.3s ease';
    }
}

// Visualizacion del carrito
function actualizarVisualizacionCarrito() {
    const contenedor = document.getElementById('items-carrito-pedido');

    if (!contenedor) return;

    if (carritoActual.length === 0) {
        contenedor.innerHTML = `
            <div style="padding: 2rem; text-align: center; color: var(--text-color);">
                <i class="fas fa-shopping-cart" style="font-size: 3rem; opacity: 0.3;"></i>
                <p>No hay productos agregados</p>
            </div>
        `;
        return;
    }

    contenedor.innerHTML = carritoActual.map((item, index) => `
        <div class="item-carrito-pedido" style="padding: 1rem; border-bottom: 1px solid var(--secondary-color); display: flex; justify-content: space-between; align-items: center; gap: 1rem;">
            <div style="flex: 1;">
                <strong style="color: var(--dark-color);">${item.nombre}</strong>
                <div style="font-size: 0.85rem; color: var(--text-color); margin-top: 0.25rem;">
                    Precio: ₡${item.precio.toFixed(2)} | Stock: ${item.stock}
                </div>
            </div>
            
            <div style="display: flex; align-items: center; gap: 0.5rem;">
                <button onclick="cambiarCantidadItem(${index}, -1)" 
                        style="width: 30px; height: 30px; border-radius: 50%; background: var(--secondary-color); border: none; cursor: pointer;">
                    -
                </button>
                <input type="number" 
                       value="${item.cantidad}" 
                       min="1" 
                       max="${item.stock}"
                       onchange="actualizarCantidadItem(${index}, this.value)"
                       style="width: 60px; text-align: center; border: 2px solid var(--secondary-color); border-radius: 5px; padding: 0.25rem;">
                <button onclick="cambiarCantidadItem(${index}, 1)" 
                        style="width: 30px; height: 30px; border-radius: 50%; background: var(--secondary-color); border: none; cursor: pointer;">
                    +
                </button>
            </div>
            
            <div style="text-align: right; min-width: 120px;">
                <div style="color: var(--primary-color); font-weight: 600; font-size: 1.1rem;">
                    ₡${(item.precio * item.cantidad).toFixed(2)}
                </div>
            </div>
            
            <button onclick="eliminarItemCarrito(${index})" 
                    style="width: 35px; height: 35px; border-radius: 50%; background: #e74c3c; color: white; border: none; cursor: pointer;">
                <i class="fas fa-trash"></i>
            </button>
        </div>
    `).join('');
}

// Cambiar cantidad de un item
function cambiarCantidadItem(index, cambio) {
    const item = carritoActual[index];
    const nuevaCantidad = item.cantidad + cambio;

    if (nuevaCantidad < 1) {
        if (confirm(`¿Desea eliminar "${item.nombre}" del carrito?`)) {
            eliminarItemCarrito(index);
        }
        return;
    }

    if (nuevaCantidad > item.stock) {
        mostrarMensaje(`Stock máximo: ${item.stock} unidades`, 'warning');
        return;
    }

    item.cantidad = nuevaCantidad;
    actualizarVisualizacionCarrito();
    calcularTotalesEnVivo();
}

// Actualizar cantidad directamente
function actualizarCantidadItem(index, nuevaCantidad) {
    nuevaCantidad = parseInt(nuevaCantidad);
    const item = carritoActual[index];

    if (isNaN(nuevaCantidad) || nuevaCantidad < 1) {
        mostrarMensaje('Cantidad mínima: 1', 'warning');
        item.cantidad = 1;
    } else if (nuevaCantidad > item.stock) {
        mostrarMensaje(`Stock máximo: ${item.stock}`, 'warning');
        item.cantidad = item.stock;
    } else {
        item.cantidad = nuevaCantidad;
    }

    actualizarVisualizacionCarrito();
    calcularTotalesEnVivo();
}

// Eliminar item del carrito
function eliminarItemCarrito(index) {
    carritoActual.splice(index, 1);
    actualizarVisualizacionCarrito();
    calcularTotalesEnVivo();
    mostrarMensaje('Producto eliminado', 'info');
}

// Contador en el navbar
function actualizarContadorCarrito() {
    fetch('/Carrito/ObtenerCantidadProductos')
        .then(response => response.json())
        .then(data => {
            const contador = document.querySelector('.cart-count');
            if (contador) {
                contador.textContent = data.cantidad || 0;
            }
        })
        .catch(error => console.error('Error al actualizar contador:', error));
}

// Mensajes de notificación
function mostrarMensaje(mensaje, tipo = 'info') {
    // Usar SweetAlert2 si está disponible
    if (typeof Swal !== 'undefined') {
        const iconos = {
            success: 'success',
            error: 'error',
            warning: 'warning',
            info: 'info'
        };

        Swal.fire({
            icon: iconos[tipo] || 'info',
            title: mensaje,
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
    } else {
        // Fallback con alerta nativa
        alert(mensaje);
    }
}