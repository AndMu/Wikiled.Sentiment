using System;
using System.Collections.Generic;
using System.IO;
using Wikiled.Core.Utility.Extensions;
using Wikiled.Core.Utility.Helpers;
using Wikiled.Core.Utility.Serialization;
using Wikiled.Sentiment.Analysis.Amazon;
using Wikiled.Text.Analysis.Structure;

namespace Wikiled.Sentiment.ConsoleApp.Unpacking
{
    public class AmazonParserLogic
    {
        public ProductCategory Category { get; set; }

        public int? MinPrice { get; set; }

        public string Product { get; set; }

        public IEnumerable<AmazonReview> Parse(string file)
        {
            DateTime? currentDate = null;

            double? stars = null;

            double? price = null;

            int? helpfulnessTotal = null;

            int? helpfulnessPositive = null;

            string currentProduct = null;

            string currentUserId = null;

            string currentUser = null;

            string productId = null;

            string summary = null;

            int rawTime = 0;

            var lines = File.ReadLines(file);
            foreach (var line in lines)
            {
                if (line.IndexOf("review/text:") == 0)
                {
                    if (MinPrice.HasValue &&
                        (!price.HasValue || price < MinPrice))
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(Product) &&
                        currentProduct.IndexOf(Product, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }

                    var text = line.Substring("review/text:".Length).Trim();
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    text = text.SanitizeXmlString();
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    var currentDocument = new Document(text);
                    currentDocument.DocumentTime = currentDate;
                    currentDocument.Stars = stars;
                    currentDocument.Title = currentProduct;
                    currentDocument.Id = currentUserId;

                    if (currentUser != null)
                    {
                        currentDocument.Author = currentUser.CreatePureLetterText();
                    }

                    AmazonReviewData reviewData = new AmazonReviewData();
                    AmazonTextData reviewTextData = new AmazonTextData();
                    ProductData product = new ProductData();
                    UserData user = new UserData();
                    user.Id = currentUserId;
                    user.Name = currentUser;

                    product.Id = productId;
                    product.Category = Category;
                    product.Price = price;
                    product.Name = currentProduct;

                    reviewData.Helpfulness = helpfulnessPositive;
                    reviewData.TotalHelpfulnessVotes = helpfulnessTotal;
                    reviewData.Score = stars.Value;
                    reviewTextData.Summary = summary;
                    reviewTextData.Text = text;
                    reviewData.Time = rawTime;
                    reviewData.Date = AmazonReviewExtension.UnixTimeStampToDateTime(rawTime);

                    currentDate = null;
                    stars = null;
                    price = null;
                    helpfulnessPositive = null;
                    helpfulnessTotal = null;
                    currentProduct = null;
                    currentUserId = null;
                    currentUser = null;
                    var review = AmazonReview.ContructNew(product, user, reviewData, reviewTextData);
                    yield return review;
                }
                else if (line.IndexOf("product/productId:") == 0)
                {
                    productId = line.Substring("product/productId:".Length).Trim();
                }
                else if (line.IndexOf("review/summary:") == 0)
                {
                    summary = line.Substring("review/summary:".Length).Trim();
                }
                else if (line.IndexOf("review/userId:") == 0)
                {
                    currentUserId = line.Substring("review/userId:".Length).Trim();
                }
                else if (line.IndexOf("review/profileName:") == 0)
                {
                    currentUser = line.Substring("review/profileName:".Length).Trim();
                }
                else if (line.IndexOf("review/helpfulness:") == 0)
                {
                    var data = line.Substring("review/helpfulness:".Length);
                    if (data != @"0/0")
                    {
                        var result = data.Split('/');
                        helpfulnessTotal = int.Parse(result[1]);
                        helpfulnessPositive = int.Parse(result[0]);
                    }
                }

                else if (line.IndexOf("review/score:") == 0)
                {
                    stars = double.Parse(line.Substring("review/score:".Length));
                }

                else if (line.IndexOf("review/time:") == 0)
                {
                    rawTime = int.Parse(line.Substring("review/time:".Length));
                    currentDate = ((long)double.Parse(line.Substring("review/time:".Length))).FromUnixTime();
                }
                else if (line.IndexOf("product/price:") == 0)
                {
                    double priceCurrent;
                    if (double.TryParse(line.Substring("product/price:".Length), out priceCurrent))
                    {
                        price = priceCurrent;
                    }
                    else
                    {
                        price = null;
                    }
                }
                else if (line.IndexOf("product/title:") == 0)
                {
                    currentProduct = line.Substring("product/title:".Length).Trim();
                }
            }
        }
    }
}
