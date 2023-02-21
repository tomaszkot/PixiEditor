#include "pch.h"
#include "CppUnitTest.h"
#include "../PixiThumbnailProvider/PngImageProvider.h"
#include <iostream>
//#include <ofstream>
#include <vector>
#include <type_traits>
#include <array>
#include <memory>
#include <algorithm>
#include <iomanip>
#pragma comment(lib, "gdiplus.lib")

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace ThumbnailProviderTests
{
  TEST_CLASS(ThumbnailProviderTests)
  {
  public:

    //TEST_METHOD(TestPngBitmapProviderLoadFromFile)
    //{
    //  PngImageProvider pngBitmapProvider([](std::wstring line) {
    //    std::wcout << line;
    //    });
    //  auto bmp = pngBitmapProvider.LoadPngImage(LR"(images\sdf.png)");
    //  Assert::IsNotNull(bmp);
    //}

    //TEST_METHOD(TestPngBitmapProviderLoadFromStream)
    //{
    //  PngImageProvider pngBitmapProvider([](std::wstring line) {
    //    std::wcout << line;
    //    });
    //  //auto bmp = pngBitmapProvider.LoadImageByStream(LR"(images\sdf.png)");
    //  auto bmp = pngBitmapProvider.LoadPngImage(LR"(images\sdf.png)");
    //  
    //  Assert::IsNotNull(bmp);
    //}


   

    TEST_METHOD(ExtractPngFromPixi)
    {
      auto pixiFile = "images\\p1.pixi";
      
      PngImageProvider pngBitmapProvider([](std::wstring line) {
        std::wcout << line;
        });

      auto bytes = pngBitmapProvider.LoadPixiPreviewBytes(pixiFile);
      //auto streamPos = 0;

      //load_bytes<unsigned char>(pixiFile, streamPos, 21);//header
      //streamPos += 21;
      //auto previewHeader = load_bytes<unsigned char>(pixiFile, 21, 4);
      //streamPos += 4;

      //auto previewSize = BytesToInt32(previewHeader, 0, true);


      //auto bytes = load_bytes<unsigned char>(pixiFile, streamPos, previewSize);
      std::ofstream outfile("images\\p1.png", std::ios::out | std::ios::binary);
      outfile.write((const char*) & bytes[0], bytes.size());
          
      auto image = pngBitmapProvider.LoadPngImage(bytes);
      Assert::IsNotNull(image);
      pngBitmapProvider.Save(L"images\\p2.png", image);
      
    }
  };
}
