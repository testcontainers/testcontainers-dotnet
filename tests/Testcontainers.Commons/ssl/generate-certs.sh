#!/bin/bash
set -e

# Generate certificates in 'ssl' using an Alpine container.
# docker run --rm -v "$(pwd)/ssl:/ssl" -w /ssl alpine:latest sh -c "apk add --no-cache openssl && sh generate-certs.sh"

readonly SSL_DIR="$(cd "$(dirname "$0")" && pwd)"
readonly CA_DIR="$SSL_DIR/ca"
readonly SERVER_DIR="$SSL_DIR/server"
readonly CLIENT_DIR="$SSL_DIR/client"
readonly CONFIG_FILE="$SSL_DIR/server.cnf"
readonly PASSWORD="password"

mkdir -p "$CA_DIR" "$SERVER_DIR" "$CLIENT_DIR"

echo "Generating CA certificate..."
openssl genrsa -out "$CA_DIR/ca.key" 2048
openssl req -x509 -new -nodes -key "$CA_DIR/ca.key" -out "$CA_DIR/ca.crt" -sha256 -days 3650 -subj "/CN=Test CA"

echo "Generating Client certificate..."
openssl genrsa -out "$CLIENT_DIR/client.key" 2048
openssl req -new -key "$CLIENT_DIR/client.key" -out "$CLIENT_DIR/client.csr" -subj "/CN=Client"
openssl x509 -req -in "$CLIENT_DIR/client.csr" -CA "$CA_DIR/ca.crt" -CAkey "$CA_DIR/ca.key" -CAcreateserial -out "$CLIENT_DIR/client.crt" -sha256 -days 365
openssl pkcs12 -export -out "$CLIENT_DIR/client.pfx" -inkey "$CLIENT_DIR/client.key" -in "$CLIENT_DIR/client.crt" -certfile "$CA_DIR/ca.crt" -password pass:$PASSWORD

echo "Generating Server certificate..."
openssl genrsa -out "$SERVER_DIR/server.key" 2048
openssl req -new -key "$SERVER_DIR/server.key" -out "$SERVER_DIR/server.csr" -config "$CONFIG_FILE"
openssl x509 -req -in "$SERVER_DIR/server.csr" -CA "$CA_DIR/ca.crt" -CAkey "$CA_DIR/ca.key" -CAcreateserial -out "$SERVER_DIR/server.crt" -sha256 -days 365 -extfile "$CONFIG_FILE" -extensions v3_req
openssl pkcs12 -export -out "$SERVER_DIR/server.pfx" -inkey "$SERVER_DIR/server.key" -in "$SERVER_DIR/server.crt" -certfile "$CA_DIR/ca.crt" -password pass:$PASSWORD

rm "$CA_DIR/ca.srl"
rm "$CLIENT_DIR/client.csr"
rm "$SERVER_DIR/server.csr"

echo "Certificates generated successfully."
