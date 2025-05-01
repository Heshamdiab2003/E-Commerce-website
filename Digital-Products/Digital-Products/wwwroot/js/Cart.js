document.addEventListener('DOMContentLoaded', function () {
    // تحديث الكمية
    const qtyButtons = document.querySelectorAll('.qty-btn');
    qtyButtons.forEach(button => {
        button.addEventListener('click', function () {
            const qtySpan = this.parentElement.querySelector('.qty-value');
            let qty = parseInt(qtySpan.textContent);

            if (this.textContent === '+') {
                qty++;
            } else if (this.textContent === '-' && qty > 1) {
                qty--;
            }

            qtySpan.textContent = qty;
            updateTotal();
        });
    });

    // حذف المنتج
    const removeButtons = document.querySelectorAll('.remove-btn');
    removeButtons.forEach(button => {
        button.addEventListener('click', function () {
            const cartItem = this.closest('.cart-item');
            cartItem.remove();
            updateTotal();
        });
    });

    // مشاركة المنتج
    const shareLinks = document.querySelectorAll('.cart-item-action-link');
    shareLinks.forEach(link => {
        if (link.textContent === 'مشاركة') {
            link.addEventListener('click', function () {
                const productTitle = this.closest('.cart-item').querySelector('.cart-item-title').textContent;
                alert(`تم نسخ رابط المنتج: ${productTitle}`);
            });
        }
    });

    // حفظ لوقت لاحق
    shareLinks.forEach(link => {
        if (link.textContent === 'حفظ لوقت لاحق') {
            link.addEventListener('click', function () {
                const productTitle = this.closest('.cart-item').querySelector('.cart-item-title').textContent;
                alert(`تم حفظ المنتج: ${productTitle} لوقت لاحق`);
            });
        }
    });

    // حذف المنتج من رابط الحذف
    shareLinks.forEach(link => {
        if (link.textContent === 'حذف') {
            link.addEventListener('click', function () {
                const cartItem = this.closest('.cart-item');
                cartItem.remove();
                updateTotal();
            });
        }
    });

    // تحديث المجموع الكلي
    function updateTotal() {
        const cartItems = document.querySelectorAll('.cart-item');
        let total = 0;
        let itemCount = 0;

        cartItems.forEach(item => {
            const price = parseFloat(item.querySelector('.cart-item-price').textContent.replace('جنيه', '').trim());
            const qty = parseInt(item.querySelector('.qty-value').textContent);
            total += price * qty;
            itemCount += qty;
        });

        const summaryTotal = document.querySelector('.cart-summary-total');
        const summaryDetails = document.querySelector('.cart-summary-details span:first-child');

        summaryTotal.textContent = `${total.toFixed(2)} جنيه`;
        summaryDetails.textContent = `الإجمالي الفرعي (${itemCount} من السلع):`;
    }

    // زر إتمام الشراء
    const checkoutBtn = document.querySelector('.checkout-btn');
    checkoutBtn.addEventListener('click', function () {
        alert('سيتم توجيهك إلى صفحة إتمام الشراء');
    });
}); 