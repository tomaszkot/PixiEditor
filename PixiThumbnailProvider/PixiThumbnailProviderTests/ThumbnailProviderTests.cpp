#include "pch.h"
#include "CppUnitTest.h"
#include "../PixiThumbnailProvider/PngImageProvider.h"
#include <iostream>
#pragma comment(lib, "gdiplus.lib")

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace ThumbnailProviderTests
{
	TEST_CLASS(ThumbnailProviderTests)
	{
	public:
		
		TEST_METHOD(TestPngBitmapProviderLoadFromFile)
		{
            PngImageProvider pngBitmapProvider([](std::wstring line) {
                std::wcout << line;
            });
            auto bmp = pngBitmapProvider.LoadPngImage(LR"(images\sdf.png)");
            Assert::IsNotNull(bmp);
		}

        TEST_METHOD(TestPngBitmapProviderLoadFromStream)
        {
          PngImageProvider pngBitmapProvider([](std::wstring line) {
                std::wcout << line;
                });
            auto bmp = pngBitmapProvider.LoadImageByStream(LR"(images\sdf.png)");
            Assert::IsNotNull(bmp);
        }
	};
}
