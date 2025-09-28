#!/usr/bin/env bash
# localstack-manage.sh
# Simple LocalStack S3 CLI using curl (no AWS CLI needed)

ENDPOINT="${LOCALSTACK_ENDPOINT:-http://localhost:4566}"
HOST_HEADER="s3.amazonaws.com"

list_buckets() {
  echo "Buckets:"
  curl -s "${ENDPOINT}/" -H "Host: ${HOST_HEADER}" \
    | xmllint --xpath "//*[local-name()='Name']/text()" - 2>/dev/null \
    | tr ' ' '\n' || echo "No buckets"
}

create_bucket() {
  bucket=$1
  if [ -z "$bucket" ]; then
    echo "Usage: $0 create-bucket <bucket-name>"
    exit 1
  fi
  curl -s -X PUT "${ENDPOINT}/${bucket}" -H "Host: ${HOST_HEADER}"
  echo "Created bucket: $bucket"
}

delete_bucket() {
  bucket=$1
  if [ -z "$bucket" ]; then
    echo "Usage: $0 delete-bucket <bucket-name>"
    exit 1
  fi
  curl -s -X DELETE "${ENDPOINT}/${bucket}" -H "Host: ${HOST_HEADER}"
  echo "Deleted bucket: $bucket"
}

list_files() {
  bucket=$1
  if [ -z "$bucket" ]; then
    echo "Usage: $0 list-files <bucket-name>"
    exit 1
  fi
  echo "Files in bucket '$bucket':"
  curl -s "${ENDPOINT}/${bucket}" -H "Host: ${HOST_HEADER}" \
    | xmllint --xpath "//*[local-name()='Key']/text()" - 2>/dev/null \
    | tr ' ' '\n' || echo "No files"
}

delete_file() {
  bucket=$1
  key=$2
  if [ -z "$bucket" ] || [ -z "$key" ]; then
    echo "Usage: $0 delete-file <bucket-name> <file-key>"
    exit 1
  fi
  curl -s -X DELETE "${ENDPOINT}/${bucket}/${key}" -H "Host: ${HOST_HEADER}"
  echo "Deleted file: $key from bucket: $bucket"
}

put_file() {
  bucket=$1
  file_path=$2
  if [ -z "$bucket" ] || [ -z "$file_path" ]; then
    echo "Usage: $0 put-file <bucket-name> <file-path>"
    exit 1
  fi
  key=$(basename "$file_path")
  curl -s -X PUT --data-binary @"${file_path}" "${ENDPOINT}/${bucket}/${key}" \
    -H "Host: ${HOST_HEADER}"
  echo "Uploaded file: $key to bucket: $bucket"
}

get_file() {
  bucket=$1
  key=$2
  output=$3
  if [ -z "$bucket" ] || [ -z "$key" ]; then
    echo "Usage: $0 get-file <bucket-name> <file-key> [output-path]"
    exit 1
  fi
  if [ -z "$output" ]; then
    output="$key"
  fi
  curl -s "${ENDPOINT}/${bucket}/${key}" -H "Host: ${HOST_HEADER}" -o "${output}"
  echo "Downloaded file: $key from bucket: $bucket to $output"
}

case "$1" in
  list-buckets) list_buckets ;;
  create-bucket) create_bucket "$2" ;;
  delete-bucket) delete_bucket "$2" ;;
  list-files) list_files "$2" ;;
  delete-file) delete_file "$2" "$3" ;;
  put-file) put_file "$2" "$3" ;;
  get-file) get_file "$2" "$3" "$4" ;;
  *)
    echo "Usage: $0 {list-buckets|create-bucket <name>|delete-bucket <name>|list-files <bucket>|delete-file <bucket> <key>|put-file <bucket> <path>|get-file <bucket> <key> [output]}"
    exit 1
    ;;
esac
