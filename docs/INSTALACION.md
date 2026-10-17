# Instalación y compilación

## Usar el ejecutable
1. Descarga `NexoraPlayout-win-x64.zip` desde la pestaña **Actions** (artefacto) o **Releases** del repositorio.
2. Descomprime y ejecuta `NexoraPlayout.exe`. No requiere instalar .NET (es autocontenido).

## FFmpeg / ffprobe (necesario para leer duraciones)
NEXORA no incluye FFmpeg dentro del repositorio. Descárgalo desde https://ffmpeg.org/download.html (compilación para Windows) y coloca `ffmpeg.exe` y `ffprobe.exe` en una carpeta llamada `ffmpeg` junto a `NexoraPlayout.exe`:

```
NexoraPlayout.exe
ffmpeg\ffprobe.exe
ffmpeg\ffmpeg.exe
```
También funciona si están en el PATH del sistema. Revisa la licencia de la compilación elegida (ver `third-party/README.md`).

## Publicar en GitHub
1. Crea el repositorio `nexora-playout` en GitHub y sube todo el contenido de esta carpeta a la rama `main`.
2. Ve a **Actions**: el flujo `build` compila solo en cada push.
3. Para una versión publicada: crea una etiqueta, por ejemplo `git tag v0.1.0 && git push origin v0.1.0`. El `.exe` se adjuntará a una Release.
