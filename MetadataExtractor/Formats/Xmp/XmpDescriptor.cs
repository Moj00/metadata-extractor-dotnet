/*
 * Copyright 2002-2015 Drew Noakes
 *
 *    Modified by Yakov Danilov <yakodani@gmail.com> for Imazen LLC (Ported from Java to C#)
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * More information about this project is available at:
 *
 *    https://drewnoakes.com/code/exif/
 *    https://github.com/drewnoakes/metadata-extractor
 */

using System;
using JetBrains.Annotations;
using MetadataExtractor.Util;
using Sharpen;

namespace MetadataExtractor.Formats.Xmp
{
    /// <summary>Contains all logic for the presentation of xmp data, as stored in Xmp-Segment.</summary>
    /// <remarks>
    /// Contains all logic for the presentation of xmp data, as stored in Xmp-Segment.  Use
    /// this class to provide human-readable descriptions of tag values.
    /// </remarks>
    /// <author>Torsten Skadell, Drew Noakes https://drewnoakes.com</author>
    public sealed class XmpDescriptor : TagDescriptor<XmpDirectory>
    {
        [NotNull]
        private static readonly DecimalFormat SimpleDecimalFormatter = new DecimalFormat("0.#");

        public XmpDescriptor([NotNull] XmpDirectory directory)
            : base(directory)
        {
        }

        // TODO some of these methods look similar to those found in Exif*Descriptor... extract common functionality from both
        /// <summary>Do some simple formatting, dependant upon tagType</summary>
        public override string GetDescription(int tagType)
        {
            switch (tagType)
            {
                case XmpDirectory.TagMake:
                case XmpDirectory.TagModel:
                {
                    return Directory.GetString(tagType);
                }

                case XmpDirectory.TagExposureTime:
                {
                    return GetExposureTimeDescription();
                }

                case XmpDirectory.TagExposureProgram:
                {
                    return GetExposureProgramDescription();
                }

                case XmpDirectory.TagShutterSpeed:
                {
                    return GetShutterSpeedDescription();
                }

                case XmpDirectory.TagFNumber:
                {
                    return GetFNumberDescription();
                }

                case XmpDirectory.TagLens:
                case XmpDirectory.TagLensInfo:
                case XmpDirectory.TagCameraSerialNumber:
                case XmpDirectory.TagFirmware:
                {
                    return Directory.GetString(tagType);
                }

                case XmpDirectory.TagFocalLength:
                {
                    return GetFocalLengthDescription();
                }

                case XmpDirectory.TagApertureValue:
                {
                    return GetApertureValueDescription();
                }

                default:
                {
                    return base.GetDescription(tagType);
                }
            }
        }

        /// <summary>Do a simple formatting like ExifSubIFDDescriptor.java</summary>
        [CanBeNull]
        public string GetExposureTimeDescription()
        {
            var value = Directory.GetString(XmpDirectory.TagExposureTime);
            if (value == null)
            {
                return null;
            }
            return value + " sec";
        }

        /// <summary>This code is from ExifSubIFDDescriptor.java</summary>
        [CanBeNull]
        public string GetExposureProgramDescription()
        {
            // '1' means manual control, '2' program normal, '3' aperture priority,
            // '4' shutter priority, '5' program creative (slow program),
            // '6' program action(high-speed program), '7' portrait mode, '8' landscape mode.
            var value = Directory.GetInteger(XmpDirectory.TagExposureProgram);
            if (value == null)
            {
                return null;
            }
            switch (value)
            {
                case 1:
                {
                    return "Manual control";
                }

                case 2:
                {
                    return "Program normal";
                }

                case 3:
                {
                    return "Aperture priority";
                }

                case 4:
                {
                    return "Shutter priority";
                }

                case 5:
                {
                    return "Program creative (slow program)";
                }

                case 6:
                {
                    return "Program action (high-speed program)";
                }

                case 7:
                {
                    return "Portrait mode";
                }

                case 8:
                {
                    return "Landscape mode";
                }

                default:
                {
                    return "Unknown program (" + value + ")";
                }
            }
        }

        /// <summary>This code is from ExifSubIFDDescriptor.java</summary>
        [CanBeNull]
        public string GetShutterSpeedDescription()
        {
            var value = Directory.GetFloatObject(XmpDirectory.TagShutterSpeed);
            if (value == null)
            {
                return null;
            }
            // thanks to Mark Edwards for spotting and patching a bug in the calculation of this
            // description (spotted bug using a Canon EOS 300D)
            // thanks also to Gli Blr for spotting this bug
            if (value <= 1)
            {
                var apexPower = (float)(1 / (Math.Exp((double)value * Math.Log(2))));
                var apexPower10 = (long)Math.Round(apexPower * 10.0);
                var fApexPower = apexPower10 / 10.0f;
                return fApexPower + " sec";
            }
            else
            {
                var apexPower = (int)((Math.Exp((double)value * Math.Log(2))));
                return "1/" + apexPower + " sec";
            }
        }

        /// <summary>Do a simple formatting like ExifSubIFDDescriptor.java</summary>
        [CanBeNull]
        public string GetFNumberDescription()
        {
            var value = Directory.GetRational(XmpDirectory.TagFNumber);
            if (value == null)
            {
                return null;
            }
            return "f/" + value.DoubleValue().ToString("0.0");
        }

        /// <summary>This code is from ExifSubIFDDescriptor.java</summary>
        [CanBeNull]
        public string GetFocalLengthDescription()
        {
            var value = Directory.GetRational(XmpDirectory.TagFocalLength);
            if (value == null)
            {
                return null;
            }
            var formatter = new DecimalFormat("0.0##");
            return formatter.Format(value.DoubleValue()) + " mm";
        }

        /// <summary>This code is from ExifSubIFDDescriptor.java</summary>
        [CanBeNull]
        public string GetApertureValueDescription()
        {
            var value = Directory.GetDoubleObject(XmpDirectory.TagApertureValue);
            if (value == null)
            {
                return null;
            }
            var fStop = PhotographicConversions.ApertureToFStop((double)value);
            return "f/" + fStop.ToString("0.0");
        }
    }
}
