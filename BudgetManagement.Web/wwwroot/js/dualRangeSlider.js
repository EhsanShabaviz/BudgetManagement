window.DualRangeSlider = {
    initInputBlocking: function (startId, endId) {
        try {
            const handler = (ev) => {
                const allowed = [8, 9, 46, 37, 38, 39, 40, 36, 35];
                if (allowed.indexOf(ev.keyCode) !== -1) return;

                const key = ev.key;
                if (!key) return;

                if (/^[0-9]$/.test(key)) return; // Latin
                if (/^[\u06F0-\u06F9]$/.test(key)) return; // Persian
                if (/^[\u0660-\u0669]$/.test(key)) return; // Arabic-Indic

                ev.preventDefault();
            };

            const pasteHandler = (ev) => {
                const text = (ev.clipboardData || window.clipboardData).getData('text');
                if (!text) { ev.preventDefault(); return; }
                const cleaned = text.replace(/[\s,٬]/g, '');
                if (/^[0-9\u06F0-\u06F9\u0660-\u0669]+$/.test(cleaned)) {
                } else {
                    ev.preventDefault();
                }
            };

            const attach = (id) => {
                const el = document.getElementById(id);
                if (!el) return;
                el.addEventListener('keydown', handler);
                el.addEventListener('paste', pasteHandler);
            };

            attach(startId);
            attach(endId);
        } catch (e) {
            console.error('DualRangeSlider.initInputBlocking error', e);
        }
    },

    setValueWithCaret: function (id, newVal) {
        try {
            const el = document.getElementById(id);
            if (!el) return;
            const prevStart = el.selectionStart;
            const prevLen = el.value.length;

            el.value = newVal;

            const newLen = newVal.length;
            const diff = newLen - prevLen;
            let newPos = prevStart + diff;
            if (newPos < 0) newPos = 0;
            if (newPos > newLen) newPos = newLen;

            el.setSelectionRange(newPos, newPos);
        } catch (e) { }
    },

    setRangeValue: function (id, newVal) {
        try {
            const el = document.getElementById(id);
            if (el) el.value = newVal;
        } catch (e) { }
    }
};