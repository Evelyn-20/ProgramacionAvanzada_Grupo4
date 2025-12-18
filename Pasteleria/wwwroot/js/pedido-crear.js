(function () {

    const fmt = n =>
        Number(n || 0).toLocaleString('es-CR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });

    function recalcular() {
        let subtotal = 0, descuento = 0, impuesto = 0;

        document.querySelectorAll('[data-producto-row="1"]').forEach(row => {
            const precio = parseFloat(row.dataset.precio);
            const impuestoPct = parseFloat(row.dataset.impuesto);
            const cant = parseInt(row.querySelector('.inp-cantidad').value || 0);
            const desc = parseFloat(row.querySelector('.inp-descuento').value || 0);

            const bruto = precio * cant;
            const descOk = Math.min(Math.max(desc, 0), bruto);
            const neto = bruto - descOk;

            subtotal += neto;
            descuento += descOk;
            impuesto += neto * (impuestoPct / 100);
        });

        const total = subtotal + impuesto;

        document.getElementById('res-subtotal').textContent = `₡${fmt(subtotal)}`;
        document.getElementById('res-descuento').textContent = `₡${fmt(descuento)}`;
        document.getElementById('res-impuesto').textContent = `₡${fmt(impuesto)}`;
        document.getElementById('res-total').textContent = `₡${fmt(total)}`;

        document.getElementById('fila-descuento').style.display =
            descuento > 0 ? 'block' : 'none';
    }

    document.addEventListener('input', e => {
        if (e.target.classList.contains('inp-cantidad') ||
            e.target.classList.contains('inp-descuento')) {
            recalcular();
        }
    });

    window.PedidoCrear = {
        obtenerItems: function () {
            const items = [];
            document.querySelectorAll('[data-producto-row="1"]').forEach(row => {
                items.push({
                    IdProducto: parseInt(row.dataset.id),
                    Cantidad: parseInt(row.querySelector('.inp-cantidad').value),
                    Descuento: parseFloat(row.querySelector('.inp-descuento').value || 0)
                });
            });
            return JSON.stringify(items);
        }
    };

    document.addEventListener('DOMContentLoaded', recalcular);
})();