#include <iostream>
#include <ratio>

namesapce ratio

int main() {
    std::ratio<1, 1000000> micro = std::micro();
    std::ratio<1, 1000000000> nano = std::nano();
    std::ratio<1, 1000000000000> pico = std::pico();

    std::cout << "micro: " << micro.num << "/" << micro.den << "\n";
    std::cout << "nano: " << nano.num << "/" << nano.den << "\n";
    std::cout << "pico: " << pico.num << "/" << pico.den << "\n";

    return 0;
}
