function getCanvas(canvasId) {
    return document.getElementById(canvasId);
}

export function ensurePreserveDrawingBufferForCanvas(canvasId) {
    const canvas = getCanvas(canvasId);
    canvas.__preserveApplied = true;

    const originalGetContext = canvas.getContext.bind(canvas);

    canvas.getContext = function (type, attrs) {
        attrs = attrs || {};
        attrs.preserveDrawingBuffer = true;
        const ctx = originalGetContext(type, attrs);
        canvas.getContext = originalGetContext;

        return ctx;
    };

    return true;
}

export function captureCanvasToPng(canvasId) {
    const canvas = getCanvas(canvasId);

    const rect = canvas.getBoundingClientRect();
    const dpr = window.devicePixelRatio || 1;

    const tmp = document.createElement("canvas");
    tmp.width  = rect.width  * dpr;
    tmp.height = rect.height * dpr;

    const ctx = tmp.getContext("2d");
    ctx.scale(dpr, dpr);
    ctx.drawImage(canvas, 0, 0);

    return tmp.toDataURL("image/png");
}
