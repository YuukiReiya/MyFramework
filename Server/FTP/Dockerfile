FROM stilliard/pure-ftpd

# e.g. you could change the defult command run:
#CMD /run.sh -c 30 -C 10 -l puredb:/etc/pure-ftpd/pureftpd.pdb -E -j -R -P $PUBLICHOST -p 30000:30059

# Imageのbuild
# docker build --rm -t my-pure-ftp .

#
#docker run -d --name ftpd_server -p 21:21 -p 30000-30009:30000-30009 -e "PUBLICHOST=localhost" stilliard/pure-ftpd


# ------

# EXPOSEで書いた場合
# Container内のポートに対してホストのポートは指定してないので勝手に割り当てられ接続される.
#EXPOSE 21:21
EXPOSE 21
#EXPOSE 3000-3009:3000-3009
EXPOSE 3000-3009

# 環境変数.
#ENV HOSTNAME
ENV PUBLICHOST=127.0.0.1
ENV FTP_USER_NAME=admin
ENV FTP_USER_PASS=admin
ENV FTP_USER_HOME=/var/ftp/

# マウント設定.
VOLUME /mnt/=/var/mnt/

# バッシュ
CMD ["bin/bash"]