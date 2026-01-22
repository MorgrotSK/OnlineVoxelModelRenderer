// wwwroot/worldInput.js
let host = null;
let dotnet = null;

function requestLock() {
    if (!host) return;
    if (document.pointerLockElement === host) return;
    host.requestPointerLock();
}

function onPointerLockChange() {
    const locked = document.pointerLockElement === host;
    if (dotnet) dotnet.invokeMethodAsync("OnPointerLockChanged", locked);
}

function onMouseMove(e) {
    if (document.pointerLockElement !== host) return;
    if (dotnet) dotnet.invokeMethodAsync("OnMouseDelta", e.movementX || 0, e.movementY || 0);
}

function onWheel(e) {
    if (host && host.contains(e.target)) {
        e.preventDefault();
        if (dotnet) dotnet.invokeMethodAsync("OnWheel", e.deltaY || 0);
    }
}

function onContextMenu(e) {
    if (host && host.contains(e.target)) e.preventDefault();
}

export function init(element, dotnetRef) {
    host = element;
    dotnet = dotnetRef;

    host.addEventListener("click", requestLock);

    document.addEventListener("pointerlockchange", onPointerLockChange);
    document.addEventListener("mousemove", onMouseMove, { passive: true });

    document.addEventListener("contextmenu", onContextMenu, { passive: false });
    document.addEventListener("wheel", onWheel, { passive: false });
}

export function dispose() {
    if (host) host.removeEventListener("click", requestLock);

    document.removeEventListener("pointerlockchange", onPointerLockChange);
    document.removeEventListener("mousemove", onMouseMove);

    document.removeEventListener("contextmenu", onContextMenu);
    document.removeEventListener("wheel", onWheel);

    host = null;
    dotnet = null;
}
