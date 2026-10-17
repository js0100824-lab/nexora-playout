# NEXORA PLAYOUT

Software de automatización televisiva para Windows (playout) con salida hacia OBS Studio 32.2.2.

## Estado del proyecto

| Elemento | Estado |
|---|---|
| Código fuente en GitHub | Etapa 2 entregada (pendiente de subir al repositorio) |
| Compilación | **No realizada todavía** (el código no se ha compilado nunca) |
| Ejecutable descargable | No existe |
| Probado en Windows | No |

### Qué incluye la Etapa 2
- Interfaz base con monitores PROGRAM/PREVIEW, temporizadores y lista (sin señal de vídeo todavía).
- Reloj en tiempo real.
- Biblioteca: añadir carpetas o archivos, arrastrar y soltar, quitar de la lista sin borrar el original, reordenar, duración (requiere ffprobe), detección de archivos movidos o eliminados.
- Guardado automático de la lista en `%APPDATA%\NexoraPlayout\playlist.json` y registro en `%APPDATA%\NexoraPlayout\logs`.
- Flujo de GitHub Actions que compila en Windows y publica el `.exe` portable.

### Qué NO está implementado todavía
- Reproducción, temporizadores de emisión, controles de transporte (Etapa 3).
- Cortes publicitarios y reanudación exacta (Etapa 4).
- Logotipo y marca de agua (Etapa 5).
- Salida hacia OBS (Etapa 6).

## Estructura

```
src/Nexora.Core      modelos (MediaItem, Playlist)
src/Nexora.Library   detección de vídeos, ffprobe
src/Nexora.Storage   JSON de configuración y registro de errores
src/Nexora.App       interfaz WPF (genera NexoraPlayout.exe)
```
Se añadirán `Nexora.Engine`, `Nexora.Ads`, `Nexora.Branding` y `Nexora.Output` en las siguientes etapas.

## Compilar en tu equipo (Windows 10/11)

1. Instala el SDK de .NET 8.
2. En la raíz del repositorio:
   ```
   dotnet publish src/Nexora.App/Nexora.App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
   ```
3. El ejecutable queda en `publish/NexoraPlayout.exe`.

Más detalles en `docs/INSTALACION.md`.
