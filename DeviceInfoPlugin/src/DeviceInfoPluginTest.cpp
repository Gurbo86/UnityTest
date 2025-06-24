#include <iostream>
#include "DeviceInfoPlugin.h"

int main() {
    char buffer[128];

    GetPlatform(buffer, sizeof(buffer));
    std::cout << "Platform: " << buffer << std::endl;

    GetCurrentDateTime(buffer, sizeof(buffer));
    std::cout << "Current DateTime: " << buffer << std::endl;

    GetCurrentLocale(buffer, sizeof(buffer));
    std::cout << "Current Locale: " << buffer << std::endl;

    return 0;
}
