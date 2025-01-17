﻿using System;

namespace JBig2Decoder.NETStandard
{
    public sealed class JBIG2Bitmap
    {

        private long width, height, line;
        private int bitmapNumber;
        public FastBitSet data;


        private ArithmeticDecoder arithmeticDecoder;
        private HuffmanDecoder huffmanDecoder;
        private MMRDecoder mmrDecoder;

        public JBIG2Bitmap(long width, long height, ArithmeticDecoder arithmeticDecoder, HuffmanDecoder huffmanDecoder, MMRDecoder mmrDecoder)
        {
            this.width = width;
            this.height = height;
            this.arithmeticDecoder = arithmeticDecoder;
            this.huffmanDecoder = huffmanDecoder;
            this.mmrDecoder = mmrDecoder;

            this.line = (width + 7) >> 3;

            this.data = new FastBitSet(width * height);
        }

        public void ReadBitmap(bool useMMR, int template, bool typicalPredictionGenericDecodingOn, bool useSkip, JBIG2Bitmap skipBitmap, short[] adaptiveTemplateX, short[] adaptiveTemplateY, long mmrDataLength)
        {

            if (useMMR)
            {

                //MMRDecoder mmrDecoder = MMRDecoder.getInstance();
                mmrDecoder.Reset();

                long[] referenceLine = new long[width + 2];
                long[] codingLine = new long[width + 2];
                codingLine[0] = codingLine[1] = width;

                for (int row = 0; row < height; row++)
                {

                    int i = 0;
                    for (; codingLine[i] < width; i++)
                    {
                        referenceLine[i] = codingLine[i];
                    }
                    referenceLine[i] = referenceLine[i + 1] = width;

                    long referenceI = 0;
                    long codingI = 0;
                    long a0 = 0;

                    do
                    {
                        int code1 = mmrDecoder.Get2DCode(), code2, code3;

                        switch (code1)
                        {
                            case MMRDecoder.twoDimensionalPass:
                                if (referenceLine[referenceI] < width)
                                {
                                    a0 = referenceLine[referenceI + 1];
                                    referenceI += 2;
                                }
                                break;
                            case MMRDecoder.twoDimensionalHorizontal:
                                if ((codingI & 1) != 0)
                                {
                                    code1 = 0;
                                    do
                                    {
                                        code1 += code3 = mmrDecoder.GetBlackCode();
                                    } while (code3 >= 64);

                                    code2 = 0;
                                    do
                                    {
                                        code2 += code3 = mmrDecoder.GetWhiteCode();
                                    } while (code3 >= 64);
                                }
                                else
                                {
                                    code1 = 0;
                                    do
                                    {
                                        code1 += code3 = mmrDecoder.GetWhiteCode();
                                    } while (code3 >= 64);

                                    code2 = 0;
                                    do
                                    {
                                        code2 += code3 = mmrDecoder.GetBlackCode();
                                    } while (code3 >= 64);

                                }
                                if (code1 > 0 || code2 > 0)
                                {
                                    a0 = codingLine[codingI++] = a0 + code1;
                                    a0 = codingLine[codingI++] = a0 + code2;

                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < width)
                                    {
                                        referenceI += 2;
                                    }
                                }
                                break;
                            case MMRDecoder.twoDimensionalVertical0:
                                a0 = codingLine[codingI++] = referenceLine[referenceI];
                                if (referenceLine[referenceI] < width)
                                {
                                    referenceI++;
                                }

                                break;
                            case MMRDecoder.twoDimensionalVerticalR1:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] + 1;
                                if (referenceLine[referenceI] < width)
                                {
                                    referenceI++;
                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < width)
                                    {
                                        referenceI += 2;
                                    }
                                }

                                break;
                            case MMRDecoder.twoDimensionalVerticalR2:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] + 2;
                                if (referenceLine[referenceI] < width)
                                {
                                    referenceI++;
                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < width)
                                    {
                                        referenceI += 2;
                                    }
                                }

                                break;
                            case MMRDecoder.twoDimensionalVerticalR3:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] + 3;
                                if (referenceLine[referenceI] < width)
                                {
                                    referenceI++;
                                    while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < width)
                                    {
                                        referenceI += 2;
                                    }
                                }

                                break;
                            case MMRDecoder.twoDimensionalVerticalL1:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] - 1;
                                if (referenceI > 0)
                                {
                                    referenceI--;
                                }
                                else
                                {
                                    referenceI++;
                                }

                                while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < width)
                                {
                                    referenceI += 2;
                                }

                                break;
                            case MMRDecoder.twoDimensionalVerticalL2:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] - 2;
                                if (referenceI > 0)
                                {
                                    referenceI--;
                                }
                                else
                                {
                                    referenceI++;
                                }

                                while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < width)
                                {
                                    referenceI += 2;
                                }

                                break;
                            case MMRDecoder.twoDimensionalVerticalL3:
                                a0 = codingLine[codingI++] = referenceLine[referenceI] - 3;
                                if (referenceI > 0)
                                {
                                    referenceI--;
                                }
                                else
                                {
                                    referenceI++;
                                }

                                while (referenceLine[referenceI] <= a0 && referenceLine[referenceI] < width)
                                {
                                    referenceI += 2;
                                }

                                break;
                            default:
                                if (JBIG2StreamDecoder.debug)
                                    Console.WriteLine("Illegal code in JBIG2 MMR bitmap data");

                                break;
                        }
                    } while (a0 < width);

                    codingLine[codingI++] = width;

                    for (int j = 0; codingLine[j] < width; j += 2)
                    {
                        for (long col = codingLine[j]; col < codingLine[j + 1]; col++)
                        {
                            SetPixel(col, row, 1);
                        }
                    }
                }

                if (mmrDataLength >= 0)
                {
                    mmrDecoder.SkipTo(mmrDataLength);
                }
                else
                {
                    if (mmrDecoder.Get24Bits() != 0x001001)
                    {
                        if (JBIG2StreamDecoder.debug)
                            Console.WriteLine("Missing EOFB in JBIG2 MMR bitmap data");
                    }
                }

            }
            else
            {

                //ArithmeticDecoder arithmeticDecoder = ArithmeticDecoder.getInstance();

                BitmapPointer cxPtr0 = new BitmapPointer(this), cxPtr1 = new BitmapPointer(this);
                BitmapPointer atPtr0 = new BitmapPointer(this), atPtr1 = new BitmapPointer(this), atPtr2 = new BitmapPointer(this), atPtr3 = new BitmapPointer(this);

                long ltpCX = 0;
                if (typicalPredictionGenericDecodingOn)
                {
                    switch (template)
                    {
                        case 0:
                            ltpCX = 0x3953;
                            break;
                        case 1:
                            ltpCX = 0x079a;
                            break;
                        case 2:
                            ltpCX = 0x0e3;
                            break;
                        case 3:
                            ltpCX = 0x18a;
                            break;
                    }
                }

                bool ltp = false;
                long cx, cx0, cx1, cx2;

                for (int row = 0; row < height; row++)
                {
                    if (typicalPredictionGenericDecodingOn)
                    {
                        int bit = arithmeticDecoder.DecodeBit(ltpCX, arithmeticDecoder.genericRegionStats);
                        if (bit != 0)
                        {
                            ltp = !ltp;
                        }

                        if (ltp)
                        {
                            DuplicateRow(row, row - 1);
                            continue;
                        }
                    }

                    int pixel;

                    switch (template)
                    {
                        case 0:

                            cxPtr0.SetPointer(0, row - 2);
                            cx0 = (cxPtr0.NextPixel() << 1);
                            cx0 |= cxPtr0.NextPixel();
                            //cx0 = (BinaryOperation.bit32ShiftL(cx0, 1)) | cxPtr0.nextPixel();

                            cxPtr1.SetPointer(0, row - 1);
                            cx1 = (cxPtr1.NextPixel() << 2);
                            cx1 |= (cxPtr1.NextPixel() << 1);
                            cx1 |= (cxPtr1.NextPixel());

                            //cx1 = (BinaryOperation.bit32ShiftL(cx1, 1)) | cxPtr1.nextPixel();
                            //cx1 = (BinaryOperation.bit32ShiftL(cx1, 1)) | cxPtr1.nextPixel();

                            cx2 = 0;

                            atPtr0.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);
                            atPtr1.SetPointer(adaptiveTemplateX[1], row + adaptiveTemplateY[1]);
                            atPtr2.SetPointer(adaptiveTemplateX[2], row + adaptiveTemplateY[2]);
                            atPtr3.SetPointer(adaptiveTemplateX[3], row + adaptiveTemplateY[3]);

                            for (int col = 0; col < width; col++)
                            {

                                cx = (BinaryOperation.Bit32ShiftL(cx0, 13)) | (BinaryOperation.Bit32ShiftL(cx1, 8)) | (BinaryOperation.Bit32ShiftL(cx2, 4)) | (atPtr0.NextPixel() << 3) | (atPtr1.NextPixel() << 2) | (atPtr2.NextPixel() << 1) | atPtr3.NextPixel();

                                if (useSkip && skipBitmap.GetPixel(col, row) != 0)
                                {
                                    pixel = 0;
                                }
                                else
                                {
                                    pixel = arithmeticDecoder.DecodeBit(cx, arithmeticDecoder.genericRegionStats);
                                    if (pixel != 0)
                                    {
                                        data.Set(row * width + col);
                                    }
                                }

                                cx0 = ((BinaryOperation.Bit32ShiftL(cx0, 1)) | cxPtr0.NextPixel()) & 0x07;
                                cx1 = ((BinaryOperation.Bit32ShiftL(cx1, 1)) | cxPtr1.NextPixel()) & 0x1f;
                                cx2 = ((BinaryOperation.Bit32ShiftL(cx2, 1)) | pixel) & 0x0f;
                            }
                            break;

                        case 1:

                            cxPtr0.SetPointer(0, row - 2);
                            cx0 = (cxPtr0.NextPixel() << 2);
                            cx0 |= (cxPtr0.NextPixel() << 1);
                            cx0 |= (cxPtr0.NextPixel());
                            /*cx0 = cxPtr0.nextPixel();
                            cx0 = (BinaryOperation.bit32ShiftL(cx0, 1)) | cxPtr0.nextPixel();
                            cx0 = (BinaryOperation.bit32ShiftL(cx0, 1)) | cxPtr0.nextPixel();*/

                            cxPtr1.SetPointer(0, row - 1);
                            cx1 = (cxPtr1.NextPixel() << 2);
                            cx1 |= (cxPtr1.NextPixel() << 1);
                            cx1 |= (cxPtr1.NextPixel());
                            /*cx1 = cxPtr1.nextPixel();
                            cx1 = (BinaryOperation.bit32ShiftL(cx1, 1)) | cxPtr1.nextPixel();
                            cx1 = (BinaryOperation.bit32ShiftL(cx1, 1)) | cxPtr1.nextPixel();*/

                            cx2 = 0;

                            atPtr0.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);

                            for (int col = 0; col < width; col++)
                            {

                                cx = (BinaryOperation.Bit32ShiftL(cx0, 9)) | (BinaryOperation.Bit32ShiftL(cx1, 4)) | (BinaryOperation.Bit32ShiftL(cx2, 1)) | atPtr0.NextPixel();

                                if (useSkip && skipBitmap.GetPixel(col, row) != 0)
                                {
                                    pixel = 0;
                                }
                                else
                                {
                                    pixel = arithmeticDecoder.DecodeBit(cx, arithmeticDecoder.genericRegionStats);
                                    if (pixel != 0)
                                    {
                                        data.Set(row * width + col);
                                    }
                                }

                                cx0 = ((BinaryOperation.Bit32ShiftL(cx0, 1)) | cxPtr0.NextPixel()) & 0x0f;
                                cx1 = ((BinaryOperation.Bit32ShiftL(cx1, 1)) | cxPtr1.NextPixel()) & 0x1f;
                                cx2 = ((BinaryOperation.Bit32ShiftL(cx2, 1)) | pixel) & 0x07;
                            }
                            break;

                        case 2:

                            cxPtr0.SetPointer(0, row - 2);
                            cx0 = (cxPtr0.NextPixel() << 1);
                            cx0 |= (cxPtr0.NextPixel());
                            /*cx0 = cxPtr0.nextPixel();
                            cx0 = (BinaryOperation.bit32ShiftL(cx0, 1)) | cxPtr0.nextPixel();
                            */

                            cxPtr1.SetPointer(0, row - 1);
                            cx1 = (cxPtr1.NextPixel() << 1);
                            cx1 |= (cxPtr1.NextPixel());
                            /*cx1 = cxPtr1.nextPixel();
                            cx1 = (BinaryOperation.bit32ShiftL(cx1, 1)) | cxPtr1.nextPixel();*/

                            cx2 = 0;

                            atPtr0.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);

                            for (int col = 0; col < width; col++)
                            {

                                cx = (BinaryOperation.Bit32ShiftL(cx0, 7)) | (BinaryOperation.Bit32ShiftL(cx1, 3)) | (BinaryOperation.Bit32ShiftL(cx2, 1)) | atPtr0.NextPixel();

                                if (useSkip && skipBitmap.GetPixel(col, row) != 0)
                                {
                                    pixel = 0;
                                }
                                else
                                {
                                    pixel = arithmeticDecoder.DecodeBit(cx, arithmeticDecoder.genericRegionStats);
                                    if (pixel != 0)
                                    {
                                        data.Set(row * width + col);
                                    }
                                }

                                cx0 = ((BinaryOperation.Bit32ShiftL(cx0, 1)) | cxPtr0.NextPixel()) & 0x07;
                                cx1 = ((BinaryOperation.Bit32ShiftL(cx1, 1)) | cxPtr1.NextPixel()) & 0x0f;
                                cx2 = ((BinaryOperation.Bit32ShiftL(cx2, 1)) | pixel) & 0x03;
                            }
                            break;

                        case 3:

                            cxPtr1.SetPointer(0, row - 1);
                            cx1 = (cxPtr1.NextPixel() << 1);
                            cx1 |= (cxPtr1.NextPixel());
                            /*cx1 = cxPtr1.nextPixel();
                            cx1 = (BinaryOperation.bit32ShiftL(cx1, 1)) | cxPtr1.nextPixel();
              */
                            cx2 = 0;

                            atPtr0.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);

                            for (int col = 0; col < width; col++)
                            {

                                cx = (BinaryOperation.Bit32ShiftL(cx1, 5)) | (BinaryOperation.Bit32ShiftL(cx2, 1)) | atPtr0.NextPixel();

                                if (useSkip && skipBitmap.GetPixel(col, row) != 0)
                                {
                                    pixel = 0;

                                }
                                else
                                {
                                    pixel = arithmeticDecoder.DecodeBit(cx, arithmeticDecoder.genericRegionStats);
                                    if (pixel != 0)
                                    {
                                        data.Set(row * width + col);
                                    }
                                }

                                cx1 = ((BinaryOperation.Bit32ShiftL(cx1, 1)) | cxPtr1.NextPixel()) & 0x1f;
                                cx2 = ((BinaryOperation.Bit32ShiftL(cx2, 1)) | pixel) & 0x0f;
                            }
                            break;
                    }
                }
            }
        }

        public void ReadGenericRefinementRegion(long template, bool typicalPredictionGenericRefinementOn, JBIG2Bitmap referredToBitmap, long referenceDX, long referenceDY, short[] adaptiveTemplateX, short[] adaptiveTemplateY)
        {

            BitmapPointer cxPtr0, cxPtr1, cxPtr2, cxPtr3, cxPtr4, cxPtr5, cxPtr6, typicalPredictionGenericRefinementCXPtr0, typicalPredictionGenericRefinementCXPtr1, typicalPredictionGenericRefinementCXPtr2;

            long ltpCX;
            if (template != 0)
            {
                ltpCX = 0x008;

                cxPtr0 = new BitmapPointer(this);
                cxPtr1 = new BitmapPointer(this);
                cxPtr2 = new BitmapPointer(referredToBitmap);
                cxPtr3 = new BitmapPointer(referredToBitmap);
                cxPtr4 = new BitmapPointer(referredToBitmap);
                cxPtr5 = new BitmapPointer(this);
                cxPtr6 = new BitmapPointer(this);
                typicalPredictionGenericRefinementCXPtr0 = new BitmapPointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr1 = new BitmapPointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr2 = new BitmapPointer(referredToBitmap);
            }
            else
            {
                ltpCX = 0x0010;

                cxPtr0 = new BitmapPointer(this);
                cxPtr1 = new BitmapPointer(this);
                cxPtr2 = new BitmapPointer(referredToBitmap);
                cxPtr3 = new BitmapPointer(referredToBitmap);
                cxPtr4 = new BitmapPointer(referredToBitmap);
                cxPtr5 = new BitmapPointer(this);
                cxPtr6 = new BitmapPointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr0 = new BitmapPointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr1 = new BitmapPointer(referredToBitmap);
                typicalPredictionGenericRefinementCXPtr2 = new BitmapPointer(referredToBitmap);
            }

            long cx, cx0, cx2, cx3, cx4;
            long typicalPredictionGenericRefinementCX0, typicalPredictionGenericRefinementCX1, typicalPredictionGenericRefinementCX2;
            bool ltp = false;

            for (int row = 0; row < height; row++)
            {

                if (template != 0)
                {

                    cxPtr0.SetPointer(0, row - 1);
                    cx0 = cxPtr0.NextPixel();

                    cxPtr1.SetPointer(-1, row);

                    cxPtr2.SetPointer(-referenceDX, row - 1 - referenceDY);

                    cxPtr3.SetPointer(-1 - referenceDX, row - referenceDY);
                    cx3 = cxPtr3.NextPixel();
                    cx3 = (BinaryOperation.Bit32ShiftL(cx3, 1)) | cxPtr3.NextPixel();

                    cxPtr4.SetPointer(-referenceDX, row + 1 - referenceDY);
                    cx4 = cxPtr4.NextPixel();

                    typicalPredictionGenericRefinementCX0 = typicalPredictionGenericRefinementCX1 = typicalPredictionGenericRefinementCX2 = 0;

                    if (typicalPredictionGenericRefinementOn)
                    {
                        typicalPredictionGenericRefinementCXPtr0.SetPointer(-1 - referenceDX, row - 1 - referenceDY);
                        typicalPredictionGenericRefinementCX0 = typicalPredictionGenericRefinementCXPtr0.NextPixel();
                        typicalPredictionGenericRefinementCX0 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX0, 1)) | typicalPredictionGenericRefinementCXPtr0.NextPixel();
                        typicalPredictionGenericRefinementCX0 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX0, 1)) | typicalPredictionGenericRefinementCXPtr0.NextPixel();

                        typicalPredictionGenericRefinementCXPtr1.SetPointer(-1 - referenceDX, row - referenceDY);
                        typicalPredictionGenericRefinementCX1 = typicalPredictionGenericRefinementCXPtr1.NextPixel();
                        typicalPredictionGenericRefinementCX1 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX1, 1)) | typicalPredictionGenericRefinementCXPtr1.NextPixel();
                        typicalPredictionGenericRefinementCX1 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX1, 1)) | typicalPredictionGenericRefinementCXPtr1.NextPixel();

                        typicalPredictionGenericRefinementCXPtr2.SetPointer(-1 - referenceDX, row + 1 - referenceDY);
                        typicalPredictionGenericRefinementCX2 = typicalPredictionGenericRefinementCXPtr2.NextPixel();
                        typicalPredictionGenericRefinementCX2 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX2, 1)) | typicalPredictionGenericRefinementCXPtr2.NextPixel();
                        typicalPredictionGenericRefinementCX2 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX2, 1)) | typicalPredictionGenericRefinementCXPtr2.NextPixel();
                    }

                    for (int col = 0; col < width; col++)
                    {

                        cx0 = ((BinaryOperation.Bit32ShiftL(cx0, 1)) | cxPtr0.NextPixel()) & 7;
                        cx3 = ((BinaryOperation.Bit32ShiftL(cx3, 1)) | cxPtr3.NextPixel()) & 7;
                        cx4 = ((BinaryOperation.Bit32ShiftL(cx4, 1)) | cxPtr4.NextPixel()) & 3;

                        if (typicalPredictionGenericRefinementOn)
                        {
                            typicalPredictionGenericRefinementCX0 = ((BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX0, 1)) | typicalPredictionGenericRefinementCXPtr0.NextPixel()) & 7;
                            typicalPredictionGenericRefinementCX1 = ((BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX1, 1)) | typicalPredictionGenericRefinementCXPtr1.NextPixel()) & 7;
                            typicalPredictionGenericRefinementCX2 = ((BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX2, 1)) | typicalPredictionGenericRefinementCXPtr2.NextPixel()) & 7;

                            int decodeBit = arithmeticDecoder.DecodeBit(ltpCX, arithmeticDecoder.refinementRegionStats);
                            if (decodeBit != 0)
                            {
                                ltp = !ltp;
                            }
                            if (typicalPredictionGenericRefinementCX0 == 0 && typicalPredictionGenericRefinementCX1 == 0 && typicalPredictionGenericRefinementCX2 == 0)
                            {
                                SetPixel(col, row, 0);
                                continue;
                            }
                            else if (typicalPredictionGenericRefinementCX0 == 7 && typicalPredictionGenericRefinementCX1 == 7 && typicalPredictionGenericRefinementCX2 == 7)
                            {
                                SetPixel(col, row, 1);
                                continue;
                            }
                        }

                        cx = (BinaryOperation.Bit32ShiftL(cx0, 7)) | (cxPtr1.NextPixel() << 6) | (cxPtr2.NextPixel() << 5) | (BinaryOperation.Bit32ShiftL(cx3, 2)) | cx4;

                        int pixel = arithmeticDecoder.DecodeBit(cx, arithmeticDecoder.refinementRegionStats);
                        if (pixel == 1)
                        {
                            data.Set(row * width + col);
                        }
                    }

                }
                else
                {

                    cxPtr0.SetPointer(0, row - 1);
                    cx0 = cxPtr0.NextPixel();

                    cxPtr1.SetPointer(-1, row);

                    cxPtr2.SetPointer(-referenceDX, row - 1 - referenceDY);
                    cx2 = cxPtr2.NextPixel();

                    cxPtr3.SetPointer(-1 - referenceDX, row - referenceDY);
                    cx3 = cxPtr3.NextPixel();
                    cx3 = (BinaryOperation.Bit32ShiftL(cx3, 1)) | cxPtr3.NextPixel();

                    cxPtr4.SetPointer(-1 - referenceDX, row + 1 - referenceDY);
                    cx4 = cxPtr4.NextPixel();
                    cx4 = (BinaryOperation.Bit32ShiftL(cx4, 1)) | cxPtr4.NextPixel();

                    cxPtr5.SetPointer(adaptiveTemplateX[0], row + adaptiveTemplateY[0]);

                    cxPtr6.SetPointer(adaptiveTemplateX[1] - referenceDX, row + adaptiveTemplateY[1] - referenceDY);

                    typicalPredictionGenericRefinementCX0 = typicalPredictionGenericRefinementCX1 = typicalPredictionGenericRefinementCX2 = 0;
                    if (typicalPredictionGenericRefinementOn)
                    {
                        typicalPredictionGenericRefinementCXPtr0.SetPointer(-1 - referenceDX, row - 1 - referenceDY);
                        typicalPredictionGenericRefinementCX0 = typicalPredictionGenericRefinementCXPtr0.NextPixel();
                        typicalPredictionGenericRefinementCX0 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX0, 1)) | typicalPredictionGenericRefinementCXPtr0.NextPixel();
                        typicalPredictionGenericRefinementCX0 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX0, 1)) | typicalPredictionGenericRefinementCXPtr0.NextPixel();

                        typicalPredictionGenericRefinementCXPtr1.SetPointer(-1 - referenceDX, row - referenceDY);
                        typicalPredictionGenericRefinementCX1 = typicalPredictionGenericRefinementCXPtr1.NextPixel();
                        typicalPredictionGenericRefinementCX1 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX1, 1)) | typicalPredictionGenericRefinementCXPtr1.NextPixel();
                        typicalPredictionGenericRefinementCX1 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX1, 1)) | typicalPredictionGenericRefinementCXPtr1.NextPixel();

                        typicalPredictionGenericRefinementCXPtr2.SetPointer(-1 - referenceDX, row + 1 - referenceDY);
                        typicalPredictionGenericRefinementCX2 = typicalPredictionGenericRefinementCXPtr2.NextPixel();
                        typicalPredictionGenericRefinementCX2 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX2, 1)) | typicalPredictionGenericRefinementCXPtr2.NextPixel();
                        typicalPredictionGenericRefinementCX2 = (BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX2, 1)) | typicalPredictionGenericRefinementCXPtr2.NextPixel();
                    }

                    for (int col = 0; col < width; col++)
                    {

                        cx0 = ((BinaryOperation.Bit32ShiftL(cx0, 1)) | cxPtr0.NextPixel()) & 3;
                        cx2 = ((BinaryOperation.Bit32ShiftL(cx2, 1)) | cxPtr2.NextPixel()) & 3;
                        cx3 = ((BinaryOperation.Bit32ShiftL(cx3, 1)) | cxPtr3.NextPixel()) & 7;
                        cx4 = ((BinaryOperation.Bit32ShiftL(cx4, 1)) | cxPtr4.NextPixel()) & 7;

                        if (typicalPredictionGenericRefinementOn)
                        {
                            typicalPredictionGenericRefinementCX0 = ((BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX0, 1)) | typicalPredictionGenericRefinementCXPtr0.NextPixel()) & 7;
                            typicalPredictionGenericRefinementCX1 = ((BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX1, 1)) | typicalPredictionGenericRefinementCXPtr1.NextPixel()) & 7;
                            typicalPredictionGenericRefinementCX2 = ((BinaryOperation.Bit32ShiftL(typicalPredictionGenericRefinementCX2, 1)) | typicalPredictionGenericRefinementCXPtr2.NextPixel()) & 7;

                            int decodeBit = arithmeticDecoder.DecodeBit(ltpCX, arithmeticDecoder.refinementRegionStats);
                            if (decodeBit == 1)
                            {
                                ltp = !ltp;
                            }
                            if (typicalPredictionGenericRefinementCX0 == 0 && typicalPredictionGenericRefinementCX1 == 0 && typicalPredictionGenericRefinementCX2 == 0)
                            {
                                SetPixel(col, row, 0);
                                continue;
                            }
                            else if (typicalPredictionGenericRefinementCX0 == 7 && typicalPredictionGenericRefinementCX1 == 7 && typicalPredictionGenericRefinementCX2 == 7)
                            {
                                SetPixel(col, row, 1);
                                continue;
                            }
                        }

                        cx = (BinaryOperation.Bit32ShiftL(cx0, 11)) | (cxPtr1.NextPixel() << 10) | (BinaryOperation.Bit32ShiftL(cx2, 8)) | (BinaryOperation.Bit32ShiftL(cx3, 5)) | (BinaryOperation.Bit32ShiftL(cx4, 2)) | (cxPtr5.NextPixel() << 1) | cxPtr6.NextPixel();

                        int pixel = arithmeticDecoder.DecodeBit(cx, arithmeticDecoder.refinementRegionStats);
                        if (pixel == 1)
                        {
                            SetPixel(col, row, 1);
                        }
                    }
                }
            }
        }

        public void ReadTextRegion(bool huffman, bool symbolRefine, long noOfSymbolInstances, long logStrips, long noOfSymbols, long[,] symbolCodeTable, long symbolCodeLength, JBIG2Bitmap[] symbols, int defaultPixel, int combinationOperator, bool transposed, int referenceCorner, int sOffset, long[,] huffmanFSTable, long[,] huffmanDSTable, long[,] huffmanDTTable, long[,] huffmanRDWTable, long[,] huffmanRDHTable, long[,] huffmanRDXTable, long[,] huffmanRDYTable, long[,] huffmanRSizeTable, int template, short[] symbolRegionAdaptiveTemplateX,
            short[] symbolRegionAdaptiveTemplateY, JBIG2StreamDecoder decoder)
        {

            JBIG2Bitmap symbolBitmap;

            int strips = 1 << (int)logStrips;

            Clear(defaultPixel);

            long t;
            if (huffman)
            {
                t = huffmanDecoder.DecodeInt(huffmanDTTable).IntResult();
            }
            else
            {
                t = arithmeticDecoder.DecodeInt(arithmeticDecoder.iadtStats).IntResult();
            }
            t *= -strips;

            int currentInstance = 0;
            long firstS = 0;
            long dt, tt, ds, s;
            while (currentInstance < noOfSymbolInstances)
            {

                if (huffman)
                {
                    dt = huffmanDecoder.DecodeInt(huffmanDTTable).IntResult();
                }
                else
                {
                    dt = arithmeticDecoder.DecodeInt(arithmeticDecoder.iadtStats).IntResult();
                }
                t += dt * strips;

                if (huffman)
                {
                    ds = huffmanDecoder.DecodeInt(huffmanFSTable).IntResult();
                }
                else
                {
                    ds = arithmeticDecoder.DecodeInt(arithmeticDecoder.iafsStats).IntResult();
                }
                firstS += ds;
                s = firstS;

                while (true)
                {

                    if (strips == 1)
                    {
                        dt = 0;
                    }
                    else if (huffman)
                    {
                        dt = decoder.ReadBits(logStrips);
                    }
                    else
                    {
                        dt = arithmeticDecoder.DecodeInt(arithmeticDecoder.iaitStats).IntResult();
                    }
                    tt = t + dt;

                    long symbolID;
                    if (huffman)
                    {
                        if (symbolCodeTable != null)
                        {
                            symbolID = huffmanDecoder.DecodeInt(symbolCodeTable).IntResult();
                        }
                        else
                        {
                            symbolID = decoder.ReadBits(symbolCodeLength);
                        }
                    }
                    else
                    {
                        symbolID = arithmeticDecoder.DecodeIAID(symbolCodeLength, arithmeticDecoder.iaidStats);
                    }

                    if (symbolID >= noOfSymbols)
                    {
                        if (JBIG2StreamDecoder.debug)
                            Console.WriteLine("Invalid symbol number in JBIG2 text region");
                    }
                    else
                    {
                        symbolBitmap = null;

                        long ri;
                        if (symbolRefine)
                        {
                            if (huffman)
                            {
                                ri = decoder.ReadBit();
                            }
                            else
                            {
                                ri = arithmeticDecoder.DecodeInt(arithmeticDecoder.iariStats).IntResult();
                            }
                        }
                        else
                        {
                            ri = 0;
                        }
                        if (ri != 0)
                        {

                            long refinementDeltaWidth, refinementDeltaHeight, refinementDeltaX, refinementDeltaY;

                            if (huffman)
                            {
                                refinementDeltaWidth = huffmanDecoder.DecodeInt(huffmanRDWTable).IntResult();
                                refinementDeltaHeight = huffmanDecoder.DecodeInt(huffmanRDHTable).IntResult();
                                refinementDeltaX = huffmanDecoder.DecodeInt(huffmanRDXTable).IntResult();
                                refinementDeltaY = huffmanDecoder.DecodeInt(huffmanRDYTable).IntResult();

                                decoder.ConsumeRemainingBits();
                                arithmeticDecoder.Start();
                            }
                            else
                            {
                                refinementDeltaWidth = arithmeticDecoder.DecodeInt(arithmeticDecoder.iardwStats).IntResult();
                                refinementDeltaHeight = arithmeticDecoder.DecodeInt(arithmeticDecoder.iardhStats).IntResult();
                                refinementDeltaX = arithmeticDecoder.DecodeInt(arithmeticDecoder.iardxStats).IntResult();
                                refinementDeltaY = arithmeticDecoder.DecodeInt(arithmeticDecoder.iardyStats).IntResult();
                            }
                            refinementDeltaX = ((refinementDeltaWidth >= 0) ? refinementDeltaWidth : refinementDeltaWidth - 1) / 2 + refinementDeltaX;
                            refinementDeltaY = ((refinementDeltaHeight >= 0) ? refinementDeltaHeight : refinementDeltaHeight - 1) / 2 + refinementDeltaY;

                            symbolBitmap = new JBIG2Bitmap(refinementDeltaWidth + symbols[(int)symbolID].width, refinementDeltaHeight + symbols[(int)symbolID].height, arithmeticDecoder, huffmanDecoder, mmrDecoder);

                            symbolBitmap.ReadGenericRefinementRegion(template, false, symbols[(int)symbolID], refinementDeltaX, refinementDeltaY, symbolRegionAdaptiveTemplateX, symbolRegionAdaptiveTemplateY);

                        }
                        else
                        {
                            symbolBitmap = symbols[(int)symbolID];
                        }

                        long bitmapWidth = symbolBitmap.width - 1;
                        long bitmapHeight = symbolBitmap.height - 1;
                        if (transposed)
                        {
                            switch (referenceCorner)
                            {
                                case 0: // bottom left
                                    Combine(symbolBitmap, tt, s, combinationOperator);
                                    break;
                                case 1: // top left
                                    Combine(symbolBitmap, tt, s, combinationOperator);
                                    break;
                                case 2: // bottom right
                                    Combine(symbolBitmap, (tt - bitmapWidth), s, combinationOperator);
                                    break;
                                case 3: // top right
                                    Combine(symbolBitmap, (tt - bitmapWidth), s, combinationOperator);
                                    break;
                            }
                            s += bitmapHeight;
                        }
                        else
                        {
                            switch (referenceCorner)
                            {
                                case 0: // bottom left
                                    Combine(symbolBitmap, s, (tt - bitmapHeight), combinationOperator);
                                    break;
                                case 1: // top left
                                    Combine(symbolBitmap, s, tt, combinationOperator);
                                    break;
                                case 2: // bottom right
                                    Combine(symbolBitmap, s, (tt - bitmapHeight), combinationOperator);
                                    break;
                                case 3: // top right
                                    Combine(symbolBitmap, s, tt, combinationOperator);
                                    break;
                            }
                            s += bitmapWidth;
                        }
                    }

                    currentInstance++;

                    DecodeIntResult decodeIntResult;

                    if (huffman)
                    {
                        decodeIntResult = huffmanDecoder.DecodeInt(huffmanDSTable);
                    }
                    else
                    {
                        decodeIntResult = arithmeticDecoder.DecodeInt(arithmeticDecoder.iadsStats);
                    }

                    if (!decodeIntResult.BooleanResult())
                    {
                        break;
                    }

                    ds = decodeIntResult.IntResult();

                    s += sOffset + ds;
                }
            }
        }

        public void ReadTextRegion2(bool huffman, bool symbolRefine, long noOfSymbolInstances, long logStrips, long noOfSymbols, long[][] symbolCodeTable, long symbolCodeLength, JBIG2Bitmap[] symbols, int defaultPixel, int combinationOperator, bool transposed, int referenceCorner, int sOffset, long[,] huffmanFSTable, long[,] huffmanDSTable, long[,] huffmanDTTable, long[,] huffmanRDWTable, long[,] huffmanRDHTable, long[,] huffmanRDXTable, long[,] huffmanRDYTable, long[,] huffmanRSizeTable, int template, short[] symbolRegionAdaptiveTemplateX,
            short[] symbolRegionAdaptiveTemplateY, JBIG2StreamDecoder decoder)
        {

            JBIG2Bitmap symbolBitmap;

            int strips = 1 << (int)logStrips;

            Clear(defaultPixel);

            //HuffmanDecoder huffDecoder = HuffmanDecoder.getInstance();
            //ArithmeticDecoder arithmeticDecoder = ArithmeticDecoder.getInstance();

            long t;
            if (huffman)
            {
                t = huffmanDecoder.DecodeInt(huffmanDTTable).IntResult();
            }
            else
            {
                t = arithmeticDecoder.DecodeInt(arithmeticDecoder.iadtStats).IntResult();
            }
            t *= -strips;

            int currentInstance = 0;
            long firstS = 0;
            long dt, tt, ds, s;
            while (currentInstance < noOfSymbolInstances)
            {

                if (huffman)
                {
                    dt = huffmanDecoder.DecodeInt(huffmanDTTable).IntResult();
                }
                else
                {
                    dt = arithmeticDecoder.DecodeInt(arithmeticDecoder.iadtStats).IntResult();
                }
                t += dt * strips;

                if (huffman)
                {
                    ds = huffmanDecoder.DecodeInt(huffmanFSTable).IntResult();
                }
                else
                {
                    ds = arithmeticDecoder.DecodeInt(arithmeticDecoder.iafsStats).IntResult();
                }
                firstS += ds;
                s = firstS;

                while (true)
                {

                    if (strips == 1)
                    {
                        dt = 0;
                    }
                    else if (huffman)
                    {
                        dt = decoder.ReadBits(logStrips);
                    }
                    else
                    {
                        dt = arithmeticDecoder.DecodeInt(arithmeticDecoder.iaitStats).IntResult();
                    }
                    tt = t + dt;

                    long symbolID;
                    if (huffman)
                    {
                        if (symbolCodeTable != null)
                        {
                            symbolID = huffmanDecoder.DecodeInt(symbolCodeTable).IntResult();
                        }
                        else
                        {
                            symbolID = decoder.ReadBits(symbolCodeLength);
                        }
                    }
                    else
                    {
                        symbolID = arithmeticDecoder.DecodeIAID(symbolCodeLength, arithmeticDecoder.iaidStats);
                    }

                    if (symbolID >= noOfSymbols)
                    {
                        if (JBIG2StreamDecoder.debug)
                            Console.WriteLine("Invalid symbol number in JBIG2 text region");
                    }
                    else
                    {
                        symbolBitmap = null;

                        long ri;
                        if (symbolRefine)
                        {
                            if (huffman)
                            {
                                ri = decoder.ReadBit();
                            }
                            else
                            {
                                ri = arithmeticDecoder.DecodeInt(arithmeticDecoder.iariStats).IntResult();
                            }
                        }
                        else
                        {
                            ri = 0;
                        }
                        if (ri != 0)
                        {

                            long refinementDeltaWidth, refinementDeltaHeight, refinementDeltaX, refinementDeltaY;

                            if (huffman)
                            {
                                refinementDeltaWidth = huffmanDecoder.DecodeInt(huffmanRDWTable).IntResult();
                                refinementDeltaHeight = huffmanDecoder.DecodeInt(huffmanRDHTable).IntResult();
                                refinementDeltaX = huffmanDecoder.DecodeInt(huffmanRDXTable).IntResult();
                                refinementDeltaY = huffmanDecoder.DecodeInt(huffmanRDYTable).IntResult();

                                decoder.ConsumeRemainingBits();
                                arithmeticDecoder.Start();
                            }
                            else
                            {
                                refinementDeltaWidth = arithmeticDecoder.DecodeInt(arithmeticDecoder.iardwStats).IntResult();
                                refinementDeltaHeight = arithmeticDecoder.DecodeInt(arithmeticDecoder.iardhStats).IntResult();
                                refinementDeltaX = arithmeticDecoder.DecodeInt(arithmeticDecoder.iardxStats).IntResult();
                                refinementDeltaY = arithmeticDecoder.DecodeInt(arithmeticDecoder.iardyStats).IntResult();
                            }
                            refinementDeltaX = ((refinementDeltaWidth >= 0) ? refinementDeltaWidth : refinementDeltaWidth - 1) / 2 + refinementDeltaX;
                            refinementDeltaY = ((refinementDeltaHeight >= 0) ? refinementDeltaHeight : refinementDeltaHeight - 1) / 2 + refinementDeltaY;

                            symbolBitmap = new JBIG2Bitmap(refinementDeltaWidth + symbols[(int)symbolID].width, refinementDeltaHeight + symbols[(int)symbolID].height, arithmeticDecoder, huffmanDecoder, mmrDecoder);

                            symbolBitmap.ReadGenericRefinementRegion(template, false, symbols[(int)symbolID], refinementDeltaX, refinementDeltaY, symbolRegionAdaptiveTemplateX, symbolRegionAdaptiveTemplateY);

                        }
                        else
                        {
                            symbolBitmap = symbols[(int)symbolID];
                        }

                        long bitmapWidth = symbolBitmap.width - 1;
                        long bitmapHeight = symbolBitmap.height - 1;
                        if (transposed)
                        {
                            switch (referenceCorner)
                            {
                                case 0: // bottom left
                                    Combine(symbolBitmap, tt, s, combinationOperator);
                                    break;
                                case 1: // top left
                                    Combine(symbolBitmap, tt, s, combinationOperator);
                                    break;
                                case 2: // bottom right
                                    Combine(symbolBitmap, (tt - bitmapWidth), s, combinationOperator);
                                    break;
                                case 3: // top right
                                    Combine(symbolBitmap, (tt - bitmapWidth), s, combinationOperator);
                                    break;
                            }
                            s += bitmapHeight;
                        }
                        else
                        {
                            switch (referenceCorner)
                            {
                                case 0: // bottom left
                                    Combine(symbolBitmap, s, (tt - bitmapHeight), combinationOperator);
                                    break;
                                case 1: // top left
                                    Combine(symbolBitmap, s, tt, combinationOperator);
                                    break;
                                case 2: // bottom right
                                    Combine(symbolBitmap, s, (tt - bitmapHeight), combinationOperator);
                                    break;
                                case 3: // top right
                                    Combine(symbolBitmap, s, tt, combinationOperator);
                                    break;
                            }
                            s += bitmapWidth;
                        }
                    }

                    currentInstance++;

                    DecodeIntResult decodeIntResult;

                    if (huffman)
                    {
                        decodeIntResult = huffmanDecoder.DecodeInt(huffmanDSTable);
                    }
                    else
                    {
                        decodeIntResult = arithmeticDecoder.DecodeInt(arithmeticDecoder.iadsStats);
                    }

                    if (!decodeIntResult.BooleanResult())
                    {
                        break;
                    }

                    ds = decodeIntResult.IntResult();

                    s += sOffset + ds;
                }
            }
        }


        public void Clear(int defPixel)
        {
            data.SetAll(defPixel == 1);
            //data.set(0, data.size(), defPixel == 1);
        }

        public void Combine(JBIG2Bitmap bitmap, long x, long y, long combOp)
        {
            long srcWidth = bitmap.width;
            long srcHeight = bitmap.height;

            //		int maxRow = y + srcHeight;
            //		int maxCol = x + srcWidth;
            //
            //		for (int row = y; row < maxRow; row++) {
            //			for (int col = x; col < maxCol; srcCol += 8, col += 8) {
            //
            //				byte srcPixelbyte = bitmap.getPixelbyte(srcCol, srcRow);
            //				byte dstPixelbyte = getPixelbyte(col, row);
            //				byte endPixelbyte;
            //
            //				switch ((int) combOp) {
            //				case 0: // or
            //					endPixelbyte = (byte) (dstPixelbyte | srcPixelbyte);
            //					break;
            //				case 1: // and
            //					endPixelbyte = (byte) (dstPixelbyte & srcPixelbyte);
            //					break;
            //				case 2: // xor
            //					endPixelbyte = (byte) (dstPixelbyte ^ srcPixelbyte);
            //					break;
            //				case 3: // xnor
            //					endPixelbyte = (byte) ~(dstPixelbyte ^ srcPixelbyte);
            //					break;
            //				case 4: // replace
            //				default:
            //					endPixelbyte = srcPixelbyte;
            //					break;
            //				}
            //				int used = maxCol - col;
            //				if (used < 8) {
            //					// mask bits
            //					endPixelbyte = (byte) ((endPixelbyte & (0xFF >> (8 - used))) | (dstPixelbyte & (0xFF << (used))));
            //				}
            //				setPixelbyte(col, row, endPixelbyte);
            //			}
            //
            //			srcCol = 0;
            //			srcRow++;
            long minWidth = srcWidth;
            if (x + srcWidth > width)
            {
                //Should not happen but occurs sometimes because there is something wrong with halftone pics
                minWidth = width - x;
            }
            if (y + srcHeight > height)
            {
                //Should not happen but occurs sometimes because there is something wrong with halftone pics
                srcHeight = height - y;
            }

            long srcIndx = 0;
            long indx = y * width + x;
            if (combOp == 0)
            {
                if (x == 0 && y == 0 && srcHeight == height && srcWidth == width)
                {
                    for (long i = 0; i < data.w.Length; i++)
                    {
                        data.w[i] |= bitmap.data.w[i];
                    }
                }
                for (long row = y; row < y + srcHeight; row++)
                {
                    indx = row * width + x;
                    data.Or(indx, bitmap.data, srcIndx, minWidth);
                    /*for (int col = 0; col < minWidth; col++) {
                        if (bitmap.data.get(srcIndx + col)) data.set(indx);
                        //data.set(indx, bitmap.data.get(srcIndx + col) || data.get(indx));
                        indx++;
                    }*/
                    srcIndx += srcWidth;
                }
            }
            else if (combOp == 1)
            {
                if (x == 0 && y == 0 && srcHeight == height && srcWidth == width)
                {
                    for (int i = 0; i < data.w.Length; i++)
                    {
                        data.w[i] &= bitmap.data.w[i];
                    }
                }
                for (long row = y; row < y + srcHeight; row++)
                {
                    indx = row * width + x;
                    for (int col = 0; col < minWidth; col++)
                    {
                        data.Set(indx, bitmap.data.Get(srcIndx + col) && data.Get(indx));
                        indx++;
                    }
                    srcIndx += srcWidth;
                }
            }

            else if (combOp == 2)
            {
                if (x == 0 && y == 0 && srcHeight == height && srcWidth == width)
                {
                    for (int i = 0; i < data.w.Length; i++)
                    {
                        data.w[i] ^= bitmap.data.w[i];
                    }
                }
                else
                {
                    for (long row = y; row < y + srcHeight; row++)
                    {
                        indx = row * width + x;
                        for (int col = 0; col < minWidth; col++)
                        {
                            data.Set(indx, bitmap.data.Get(srcIndx + col) ^ data.Get(indx));
                            indx++;
                        }
                        srcIndx += srcWidth;
                    }
                }
            }

            else if (combOp == 3)
            {
                for (long row = y; row < y + srcHeight; row++)
                {
                    indx = row * width + x;
                    for (int col = 0; col < minWidth; col++)
                    {
                        bool srcPixel = bitmap.data.Get(srcIndx + col);
                        bool pixel = data.Get(indx);
                        data.Set(indx, pixel == srcPixel);
                        indx++;
                    }
                    srcIndx += srcWidth;
                }
            }

            else if (combOp == 4)
            {
                if (x == 0 && y == 0 && srcHeight == height && srcWidth == width)
                {
                    for (long i = 0; i < data.w.Length; i++)
                    {
                        data.w[i] = bitmap.data.w[i];
                    }
                }
                else
                {
                    for (long row = y; row < y + srcHeight; row++)
                    {
                        indx = row * width + x;
                        for (int col = 0; col < minWidth; col++)
                        {
                            data.Set(indx, bitmap.data.Get(srcIndx + col));
                            srcIndx++;
                            indx++;
                        }
                        srcIndx += srcWidth;
                    }
                }
            }
        }

        /**
         * set a full byte of pixels
         */
        //	private void setPixelbyte(int col, int row, byte bits) {
        //data.setbyte(row, col, bits);
        //	}

        /**
         * get a byte of pixels
         */
        //	public byte getPixelbyte(int col, int row) {
        //return data.getbyte(row, col);
        //	}

        private void DuplicateRow(int yDest, int ySrc)
        {
            //		for (int i = 0; i < width;) {
            //			setPixelbyte(i, yDest, getPixelbyte(i, ySrc));
            //			i += 8;
            //		}
            for (int i = 0; i < width; i++)
            {
                SetPixel(i, yDest, GetPixel(i, ySrc));
            }
        }

        public long GetWidth()
        {
            return width;
        }

        public long GetHeight()
        {
            return height;
        }

        public byte[] GetData(bool switchPixelColor)
        {
            byte[] bytes = new byte[height * line];

            long count = 0, offset = 0;
            long k = 0;
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if ((count & FastBitSet.mask) == 0)
                    {
                        k = data.w[(int)((ulong)count >> FastBitSet.pot)];
                    }
                    long bit = 7 - (offset & 0x7);
                    bytes[offset >> 3] |= (byte)(((int)((ulong)k >> (int)count) & 1) << (int)bit);
                    count++;
                    offset++;
                }

                offset = (line * 8 * (row + 1));
            }

            if (switchPixelColor)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] ^= 0xff;
                }
            }

            return bytes;
        }

        public JBIG2Bitmap GetSlice(long x, long y, long width, long height)
        {
            //		JBIG2Bitmap slice = new JBIG2Bitmap(width, height);
            //
            //		int sliceRow = 0, sliceCol = 0;
            //		int maxCol = x + width;
            //		
            //		//ShowGUIMessage.showGUIMessage("x", this.getBufferedImage(), "xx");
            //		
            //		System.out.println(">>> getSlice x = "+x+" y = "+y+ " width = "+width+ " height = "+height);
            //		System.out.println(">>> baseImage width = "+this.width+ " height = "+this.height);
            //		
            //		System.out.println("counter = "+counter);
            //		if(counter == 17){
            //			System.out.println();
            //			//ShowGUIMessage.showGUIMessage("x", this.getBufferedImage(), "xx");
            //		}
            //		
            //		ShowGUIMessage.showGUIMessage("x", this.getBufferedImage(), "xx");
            //		
            //		for (int row = y; row < height; row++) {
            //			for (int col = x; col < maxCol; col += 8, sliceCol += 8) {
            //				slice.setPixelbyte(sliceCol, sliceRow, getPixelbyte(col, row));
            //				//if(counter > 10)
            //					//ShowGUIMessage.showGUIMessage("new", slice.getBufferedImage(), "new");
            //			}
            //			sliceCol = 0;
            //			sliceRow++;
            //		}
            //		counter++;
            //
            //		ShowGUIMessage.showGUIMessage("new", slice.getBufferedImage(), "new");
            //		
            //		return slice;

            JBIG2Bitmap slice = new JBIG2Bitmap(width, height, arithmeticDecoder, huffmanDecoder, mmrDecoder);

            /*		int sliceRow = 0, sliceCol = 0;
                    for (int row = y; row < height; row++) {
                        for (int col = x; col < x + width; col++) {
                            //System.out.println("row = "+row +" column = "+col);
                            //slice.setPixel(sliceCol, sliceRow, getPixel(col, row));
                            slice.data.set(sliceRow*slice.width + sliceCol, data.get(row*this.width + col));
                            sliceCol++;
                        }
                        sliceCol = 0;
                        sliceRow++;
                    }

                    return slice;*/
            //int sliceRow = 0, sliceCol = 0;
            int sliceIndx = 0;
            for (long row = y; row < height; row++)
            {
                long indx = row * this.width + x;
                for (long col = x; col < x + width; col++)
                {
                    if (data.Get(indx)) slice.data.Set(sliceIndx);
                    sliceIndx++;
                    indx++;
                }
            }

            return slice;
        }

        /**
        private static void setPixel(int col, int row, FastBitSet data, int value) {
            if (value == 1)
                data.set(row, col);
            else
                data.clear(row, col);
        }/**/

        //	private void setPixelbyte(int col, int row, FastBitSet data, byte bits) {
        //		data.setbyte(row, col, bits);
        //	}

        //	public void setPixel(int col, int row, int value) {
        //		setPixel(col, row, data, value);
        //	}

        //	public int getPixel(int col, int row) {
        //		return data.get(row, col) ? 1 : 0;
        //	}

        private void SetPixel(long col, long row, FastBitSet data, long value)
        {
            long index = (row * width) + col;

            data.Set(index, value == 1);
        }

        public void SetPixel(long col, long row, long value)
        {
            SetPixel(col, row, data, value);
        }

        public int GetPixel(int col, int row)
        {
            return data.Get((row * width) + col) ? 1 : 0;
        }

        public void Expand(int newHeight, int defaultPixel)
        {
            FastBitSet newData = new FastBitSet(newHeight * width);
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    SetPixel(col, row, newData, GetPixel(col, row));
                }
            }

            this.height = newHeight;
            this.data = newData;
        }

        public void SetBitmapNumber(int segmentNumber)
        {
            this.bitmapNumber = segmentNumber;
        }

        public int GetBitmapNumber()
        {
            return bitmapNumber;
        }

        public byte[] GetBufferedImage()
        {
            byte[] bytes = GetData(true);

            if (bytes == null)
                return null;

            // make a a DEEP copy so we can't alter
            int len = bytes.Length;
            byte[] copy = new byte[len];
            Array.Copy(bytes, 0, copy, 0, len);

            return copy;
        }
    }
}
