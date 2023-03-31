#!/bin/bash

# replace with the path to your file
FILE_PATH="urls.txt"

# create a directory to store downloaded pages
if [ ! -d "pages" ]; then
    mkdir pages
fi

# loop through each URL and download its content
while read -r url; do
    # create a file name based on the URL
    file_name=$(echo "$url" | sed 's/https:\/\///' | tr '/' '_').html
    file_path="pages/$file_name"

    # download the page content and save it to a file
    #curl -s -o "$file_path" "$url"
    # curl -x "http://scraperapi:f0cbd5e0d2f23bb835a60135e4871c7c@proxy-server.scraperapi.com:8001" -k "$file_path"
    curl $url > "$file_path"
done < "$FILE_PATH"
