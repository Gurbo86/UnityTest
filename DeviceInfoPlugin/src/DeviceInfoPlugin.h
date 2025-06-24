#pragma once

#ifdef _WIN32
#define EXPORT_API __declspec(dllexport)
#else
#define EXPORT_API __attribute__((visibility("default")))
#endif

#ifdef __cplusplus
extern "C" {
#endif

// Consulta la plataforma actual
EXPORT_API void GetPlatform(char* buffer, int bufferSize);

// Obtiene la fecha y hora actual del dispositivo (formato ISO8601)
EXPORT_API void GetCurrentDateTime(char* buffer, int bufferSize);

// Obtiene la información de locale actual (ej: es-AR, en-US)
EXPORT_API void GetCurrentLocale(char* buffer, int bufferSize);

#ifdef __cplusplus
}
#endif
