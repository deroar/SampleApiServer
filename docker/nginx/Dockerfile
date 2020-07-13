# アプリ用の設定を取り込んだNGINX実行イメージを作成
FROM nginx AS runtime
COPY docker/nginx/nginx.conf /etc/nginx/
COPY docker/nginx/conf.d/mysite.template /etc/nginx/conf.d/
# 静的ファイルを配置
COPY docker/nginx/www /var/www
# NGINX起動時に動的に環境変数を展開する
CMD envsubst '$$NGINX_PORT $$BACKEND_SERVER'< /etc/nginx/conf.d/mysite.template > /etc/nginx/conf.d/default.conf && exec nginx -g 'daemon off;'
