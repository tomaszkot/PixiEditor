#pragma once
#include <string>
#include <locale>
#include <codecvt>
#include <iostream>
#include <fstream>
//inline std::string w2s(const std::wstring& var)
//{
//    static std::locale loc("");
//    auto& facet = std::use_facet<std::codecvt<wchar_t, char, std::mbstate_t>>(loc);
//    return std::wstring_convert<std::remove_reference<decltype(facet)>::type, wchar_t>(&facet).to_bytes(var);
//}
//
//inline  std::wstring s2w(const std::string& var)
//{
//    static std::locale loc("");
//    auto& facet = std::use_facet<std::codecvt<wchar_t, char, std::mbstate_t>>(loc);
//    return std::wstring_convert<std::remove_reference<decltype(facet)>::type, wchar_t>(&facet).from_bytes(var);
//}

int BytesToInt32(std::vector<unsigned char>& input, int startOffset, bool isLittleEndian)
{
  if (input.empty()) {
    std::cerr << "FATAL: Null input byte vector";
    return 0;
  }
  if ((unsigned int)startOffset >= input.size()) {
    std::cerr << "FATAL: Out of Range";
    return 0;
  }
  if (startOffset > input.size() - 4) {
    std::cerr << "FATAL: Input byte vector is too small";
    return 0;
  }

  // 0x00000000
  if (isLittleEndian) {       // MSB is loacted in higher address.
    return (input[startOffset]) | (input[startOffset + 1] << 8) |
      (input[startOffset + 2] << 16) | (input[startOffset + 3] << 24);
  }
  else {                    // MSB is located in lower address.
    return (input[startOffset] << 24) | (input[startOffset] << 16) |
      (input[startOffset] << 8) | (input[startOffset]);
  }
}

template<typename T>
std::vector<T> load_bytes(std::string const& filepath, int skipBytes = 0, int size = 0)
{
  std::ifstream ifs(filepath, std::ios::binary);

  if (!ifs)
    throw std::runtime_error(filepath + ": " + std::strerror(errno));

  ifs.seekg(skipBytes, std::ios::beg);

  if (size == 0)
    return {};

  std::vector<T> buffer(size);

  if (!ifs.read((char*)buffer.data(), buffer.size()))
    throw std::runtime_error(filepath + ": " + std::strerror(errno));

  return buffer;
}

