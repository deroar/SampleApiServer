using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApiServer.Util.Swagger
{
    /// <summary>
    /// Swaggerリクエストヘッダ用フィルタ
    /// </summary>
    public class AddHeaderOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 各APIで共通のSwagger定義を適用する。
        /// </summary>
        /// <param name="operation">オペレーション</param>
        /// <param name="context">コンテキスト</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            // アプリバージョンは全API共通
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-APP-VERSION",
                In = ParameterLocation.Header,
                Description = "アプリバージョン。",
                Required = true,
                Schema = new OpenApiSchema { Type = "string", Example = new OpenApiString("0.0.0") },
            });

            // ※ 以下、本番では必須だが、開発環境では無くても可。Swaggerは開発環境専用なので必須化しない。
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-MESSAGE-ID",
                In = ParameterLocation.Header,
                Description = "メッセージの固有ID。0から始め、同一セッション内でインクリメントしていく。",
                Schema = new OpenApiSchema { Type = "integer", Format = "int64" },
            });

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-MASTER-HASH",
                In = ParameterLocation.Header,
                Description = "マスタバージョンのハッシュ値。",
                Schema = new OpenApiSchema { Type = "string" },
            });
        }
    }
}
