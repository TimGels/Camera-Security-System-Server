#!/bin/sh

# Generate CA key and certificate
openssl req -x509 -nodes -new -sha512 -days 365 -newkey rsa:4096 -keyout ca.key -out ca.pem -subj "/C=NL/ST=Drenthe/L=Emmen/O=CSS"
openssl x509 -outform pem -in ca.pem -out ca.crt

# Create a certificate signing request
openssl req -new -nodes -newkey rsa:4096 -keyout cssserver.key -out cssserver.csr -subj "/C=NL/ST=Drenthe/L=Emmen/O=CSS"

# Complete the request by using the CA certificate to sign the certificate
openssl x509 -req -sha512 -days 365 -extfile v3.ext -CA ca.crt -CAkey ca.key -CAcreateserial -in cssserver.csr -out cssserver.crt
