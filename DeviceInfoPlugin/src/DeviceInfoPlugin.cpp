#include "DeviceInfoPlugin.h"
#include <string>
#include <ctime>
#include <locale>
#include <cstring>

#ifdef _WIN32
#include <windows.h>
#endif


#if defined(__ANDROID__)
#include <jni.h>
#include <android/log.h>

static JavaVM* g_JavaVM = nullptr;

extern "C" jint JNI_OnLoad(JavaVM* vm, void*)
{
    __android_log_print(ANDROID_LOG_INFO, "DeviceInfoPlugin", "JNI_OnLoad(): OnLoad Called.");
    g_JavaVM = vm;

    if (!g_JavaVM)
    {
        __android_log_print(ANDROID_LOG_ERROR, "DeviceInfoPlugin", "JNI_OnLoad(): OnLoad error. g_JavaVM is null.");
    }

    return JNI_VERSION_1_6;
}

static JNIEnv* GetEnv()
{
    if (!g_JavaVM)
    {
        __android_log_print(ANDROID_LOG_ERROR, "DeviceInfoPlugin", "GetEnv(): g_JavaVM is null. Make sure JNI_OnLoad was called.");
        return nullptr;
    }

    JNIEnv* env = nullptr;

    jint res = g_JavaVM->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6);
    
    if(res != JNI_OK)
    {
        return env;
    }

    if (res != JNI_EDETACHED)
    {
        __android_log_print(ANDROID_LOG_INFO, "DeviceInfoPlugin", "GetEnv(): thread detached, attaching…");

        // Hilo aún no adjunto → lo adjuntamos
        if (g_JavaVM->AttachCurrentThread(&env, nullptr) != JNI_OK)
        {
            __android_log_print(ANDROID_LOG_ERROR, "DeviceInfoPlugin", "GetEnv(): Try to attach to javaVM thread and Failed. javaVM=%p,", g_JavaVM);
            return nullptr;
        }

        return env;
    }
    
    __android_log_print(ANDROID_LOG_ERROR, "DeviceInfoPlugin", "GetEnv(): GetEnv error %d", res);
    return nullptr;
}
#endif


extern "C" {

    EXPORT_API void GetPlatform(char* buffer, int bufferSize) {
    #if defined(__ANDROID__)
        const char* platform = "Android";
    #elif defined(__EMSCRIPTEN__)
        const char* platform = "Web";
    #elif defined(_WIN32)
        const char* platform = "Windows";
    #elif defined(__APPLE__)
        #include <TargetConditionals.h>
        #if TARGET_OS_IPHONE
            const char* platform = "iOS";
        #else
            const char* platform = "macOS";
        #endif
    #else
        const char* platform = "Unknown";
    #endif
        strncpy(buffer, platform, bufferSize - 1);
        buffer[bufferSize - 1] = '\0';
    }

    EXPORT_API void GetCurrentDateTime(char* buffer, int bufferSize) {
        std::time_t t = std::time(nullptr);
        char temp[32];
        std::strftime(temp, sizeof(temp), "%Y-%m-%dT%H:%M:%SZ", std::gmtime(&t));
        strncpy(buffer, temp, bufferSize - 1);
        buffer[bufferSize - 1] = '\0';
    }

    EXPORT_API void GetCurrentLocale(char* buffer, int bufferSize) {
    #if defined(_WIN32)
        wchar_t localeName[LOCALE_NAME_MAX_LENGTH];
        if (GetUserDefaultLocaleName(localeName, LOCALE_NAME_MAX_LENGTH)) {
            char localeA[LOCALE_NAME_MAX_LENGTH];
            WideCharToMultiByte(CP_UTF8, 0, localeName, -1, localeA, LOCALE_NAME_MAX_LENGTH, NULL, NULL);
            strncpy(buffer, localeA, bufferSize - 1);
            buffer[bufferSize - 1] = '\0';
        } else {
            strncpy(buffer, "en-US", bufferSize - 1);
            buffer[bufferSize - 1] = '\0';
        }
    #elif defined(__ANDROID__)
        __android_log_print(ANDROID_LOG_INFO, "DeviceInfoPlugin", "GetCurrentLocale() : CALLED!");
        JNIEnv* env = GetEnv();

        if (!env) {
            __android_log_print(ANDROID_LOG_ERROR, "DeviceInfoPlugin", "GetCurrentLocale() : Failed to get JNIEnv");
            strncpy(buffer,"unknown",bufferSize);
            buffer[bufferSize - 1] = '\0';
            return;
        }

        jclass localeCls  = env->FindClass("java/util/Locale");
        jmethodID getDef  = env->GetStaticMethodID(localeCls,"getDefault","()Ljava/util/Locale;");

        jobject localeObj = env->CallStaticObjectMethod(localeCls, getDef);

        jmethodID toStr   = env->GetMethodID(localeCls,"toString","()Ljava/lang/String;");
        
        jstring localeStr = (jstring)env->CallObjectMethod(localeObj, toStr);

        const char* utf   = env->GetStringUTFChars(localeStr, nullptr);
        strncpy(buffer, utf, bufferSize - 1);
        buffer[bufferSize - 1] = '\0';

        env->ReleaseStringUTFChars(localeStr, utf);
        env->DeleteLocalRef(localeStr);
        env->DeleteLocalRef(localeObj);
        env->DeleteLocalRef(localeCls);
    #else
        strncpy(buffer, "unknown", bufferSize - 1);
        buffer[bufferSize - 1] = '\0';
    #endif
    }

} // extern "C"