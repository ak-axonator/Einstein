import os
from bs4 import BeautifulSoup
import re
folder_path = "/Users/arohikulkarni/Work/Einstein/SiteExtraction/pages"

lines = []
errors = []
verbose = []
page_name = ""
def log_url(lines, filename, idx):
    split_filename = filename.split("_")

        # Extracting the domain name from the split filename
    domain_name = split_filename[0]

        # Replacing "-" with "/" to get the correct format of the filename
    page_name = split_filename[1].replace(".html", "")

        # Combining the domain name and page name to form the final filename
    final_filename = f"https://{domain_name}/{page_name}"

    print(str(idx) +". "+ final_filename)
    # lines.append(str(idx) +  "URL: "+final_filename)
    errors.append(str(idx) +  "URL: "+final_filename)
    verbose.append(str(idx) +  "URL: "+final_filename)

    return final_filename

def find_longest_text(hero_descriptions):
    if len(hero_descriptions) == 0:
        return ""
    return max([sub.text for sub in hero_descriptions], key=len).strip()

footer_detected = False
def is_detected_after_footer(footer_detected):
    if footer_detected:
        verbose.append("This section is detected after footer section")
        #errors.append("This section is detected after footer section")
        #lines.append("This section is detected after footer section")

def log_filename(lines, errors, verbose, filename):
    lines.append("--------------------")
    verbose.append("--------------------")
    errors.append("--------------------")

    # lines.append(filename)
    verbose.append(filename)
    errors.append(filename)

for idx, filename in enumerate(os.listdir(folder_path)):
    import pdb;pdb.set_trace()
    if filename.endswith(".html"):
        #only process the follwing file
        with open(os.path.join(folder_path, filename), "r", encoding="utf8") as file:
            html = file.read()        
        soup = BeautifulSoup(html, "html.parser")
        # author = soup.find("meta", attrs={"name":"author"}).get("content")
        heading = soup.find("h1").text
        author = soup.find("span",{"class":"elementor-post-info__item--type-author"}).text.strip()
        date = soup.find("span",{"class":"elementor-post-info__item--type-date"}).text.strip()

        log_filename(lines, errors, verbose, filename)
        log_url(lines, filename, idx)
        page = soup.find("div",{"data-elementor-type":"single-post"})
        for section in page.findAll("div",{"class":"elementor-widget"}):
            if section.find('div', class_='elementor-widget-container'):

                p = section.find('div', class_='elementor-widget-container').find('p')
                if p:
                    lines.append("paragraph section found")
                    lines.append(p.text.strip()+"\n")
                else:
                    # Check if the child div contains an img tag with width > 500
                    img = section.find('div', class_='elementor-widget-container').find('img')
                    if img and int(img.get('width', 0)) > 500:
                        pdb.set_trace()
                        lines.append("single image section found")
                        lines.append("image \"" + img.get("src") + "\" alt=\"" + img.get("alt") + "\""+"\n")


    if idx == 40:
        break
with open("output.txt", "w") as file:
    file.write(str("\n".join(lines).encode('utf8')).replace("b'", "").replace("\\r", "\r").replace("\\n", "\n").replace("\\t", "\t"))
with open("errors.txt", "w") as file:
    file.write(str("\n".join(errors).encode('utf8')).replace("b'", "").replace("\\r", "\r").replace("\\n", "\n").replace("\\t", "\t"))
with open("verbose.txt", "w") as file:
    file.write(str("\n".join(verbose).encode('utf8')).replace("b'", "").replace("\\r", "\r").replace("\\n", "\n").replace("\\t", "\t"))