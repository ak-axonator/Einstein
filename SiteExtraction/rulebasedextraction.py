import os
from bs4 import BeautifulSoup
import re
folder_path = "/Users/arohikulkarni/Work/Website Project/Old Website/pages"

lines = []
errors = []
verbose = []
page_name = []
def log_url(lines, filename, idx):
    split_filename = filename.split("_")

        # Extracting the domain name from the split filename
    domain_name = split_filename[0]

        # Replacing "-" with "/" to get the correct format of the filename
    page_name.append(split_filename[1].replace(".html", ""))

        # Combining the domain name and page name to form the final filename
    final_filename = f"https://{domain_name}/{page_name[0]}"

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
    # lines.append("--------------------")
    verbose.append("--------------------")
    errors.append("--------------------")

    # lines.append(filename)
    verbose.append(filename)
    errors.append(filename)

for idx, filename in enumerate(os.listdir(folder_path)):
    if filename.endswith(".html"):
        #only process the follwing file
        if filename != "axonator.com_coronavirus-self-declaration-mobile-app.html":
           continue
        with open(os.path.join(folder_path, filename), "r", encoding="utf8") as file:
            html = file.read()        
        soup = BeautifulSoup(html, "html.parser")
       

        log_filename(lines, errors, verbose, filename)
        log_url(lines, filename, idx)

        # expect: not continuous group headers << continuous 2 header section detected
        # find hero section
        # expect: only 1 wp-page div and only one child div
        wp_page = soup.find("div", {"data-elementor-type": "wp-page"})
        if wp_page is None:
            errors.append("Error: no wp-page div")
            continue
        page_children = wp_page.findChildren("div" , recursive=False)
        hero_section_detected = False

        if len(page_children) > 1:
            errors.append("Error: more than 1 child div")
        elif len(page_children) == 0:
            errors.append("Error: no child div so no hero section detected, trying section now..")
            page_children = wp_page.findChildren("section" , recursive=False)
            if len(page_children) > 1:
                errors.append("Error: more than 1 child section")
            elif len(page_children) == 0:
                errors.append("Error: no child section so no hero section detected, aborting hero section detection, moving ahead")
            else:
                hero_section_detected = True
        else:
            hero_section_detected = True
        if hero_section_detected:
            hero_div = page_children[0]
            hero_img = hero_div.find("img")
            if hero_img is None:
                errors.append("Error: no hero image")
            else:
                hero_image_src = hero_img["src"]
                hero_image_alt = hero_img["alt"]
            hero_h1 = hero_div.find("h1")
            if hero_h1 is None:
                errors.append("Error: no hero h1")
            else:
                hero_heading = hero_h1.text.strip()
            
            hero_descriptions = hero_div.find_all("div", {"class": "elementor-widget-container"})
            hero_description = find_longest_text(hero_descriptions)
            lines.append("hero_section {")
            lines.append("\theading \"" + hero_heading + "\"")
            lines.append("\tsub_heading \"" + hero_description + "\"")
            lines.append("\timage \"" + hero_image_src + "\" alt=\"" + hero_image_alt + "\"")
            lines.append("}")

        # find all sections
        sections = soup.find_all("section")
        
        for section in sections:
            if len(section.find_all("h3")) > 0 and len(section.find_all("h4")) > 0:
                errors.append(">>>>>>Warning: h3 and h4 found in same section")

            if section.find_parent("div", class_="elementor-location-footer") is not None:
                footer_detected = True
                verbose.append("Section is inside footer section")
            elif section.find_parent("div", class_="ekit-template-content-markup") is not None:
                verbose.append("Section is inside navigation section")
            # check if its left right section; check has class: leftrightsection 
            elif "leftrightsection" in section["class"]:
                lines.append("image_section {")
                is_detected_after_footer(footer_detected)
                #lines.append heading
                lines.append("\theading \"" + section.find("h3").text.strip() + "\"")
                #lines.append image
                lines.append("\timage \"" + section.find("img")["src"] + "\"")
                # error if it contains more than 1 img
                if len(section.find_all("img")) > 1:
                    lines.append("Error: more than 1 img")
                # lines.append description
                descriptions = section.find_all("div" , {"class": "elementor-widget-container"})
                description = find_longest_text(descriptions)
                lines.append("\tdescription \"" + description + "\"")
                lines.append("}")

            # check if its a feature section: contains 6 or 3 h3
            elif len(section.find_all("h3")) == 6 or len(section.find_all("h3")) == 3:
                featured_h2 = section.find("h2")
                if featured_h2 is None:
                    errors.append("Error: no h2 found in featured section which has 6 h3")
                else:
                    # lines.append("Feature section found (h3 type): "+featured_h2.text.strip())
                    # grid_section for features found
                    lines.append("grid_section {")
                    lines.append("\theading \"" + featured_h2.text.strip() + "\"")
                    lines.append("\tsub_heading \"\"")
                    is_detected_after_footer(footer_detected)
                    # lines.append all features
                    for div in section.find_all("div", class_="elementor-col-33"):
                        featued_h3 = div.find("h3")
                        if featued_h3 is None:
                            errors.append("Error: no h3 found in feature section")
                        else:
                            lines.append("\tcard {")
                            lines.append("\t\ttitle \"" + featued_h3.text.strip() + "\"")
                        
                        all_descs = div.find_all("div", class_="elementor-widget-container")
                        description = find_longest_text(all_descs)
                        if len(description) < 30:
                            errors.append("Error: no description os samll description found in feature section: "+description)
                        else:
                            lines.append("\t\tdescription \"" + description + "\"")
                            lines.append("\t}")
                        if featued_h3:
                            if description == featued_h3.text.strip():
                                errors.append("Error: description is same as heading")
                    lines.append("}")

            # check if its a feature section: contains 6 h4
            elif len(section.find_all("h4")) == 6 or len(section.find_all("h4")) == 3:
                featured_h2 = section.find("h2")
                if featured_h2 is None:
                    errors.append("Error: no h2 found in featured section which has 6 h3")
                else:
                    lines.append("Feature section found (h4 type): "+featured_h2.text.strip())
                    # grid_section for features found
                    lines.append("grid_section {")
                    lines.append("\theading \"" + featured_h2.text.strip() + "\"")
                    lines.append("\tsub_heading \"\"")
                    is_detected_after_footer(footer_detected)
                    # lines.append all features
                    for div in section.find_all("div", class_="elementor-col-33"):
                        featued_h3 = div.find("h4")
                        if featued_h3 is None:
                            errors.append("Error: no h3 found in feature section")
                        else:
                            lines.append("\tcard {")
                            lines.append("\t\ttitle \"" + featued_h3.text.strip() + "\"")
                        
                        all_descs = div.find_all("div", class_="elementor-widget-container")
                        description = find_longest_text(all_descs)
                        if len(description) < 30:
                            errors.append("Error: no description os samll description found in feature section: "+description)
                        else:
                            lines.append("\t\tdescription \"" + description + "\"")
                            lines.append("\t}")
                        if featued_h3:
                            if description == featued_h3.text.strip():
                                errors.append("Error: description is same as heading")
                    lines.append("}")
            
            # check if its a footer section: parent has the class elementor-location-footer
            #elif footer_detected != None and "elementor-location-footer" in section.parent["class"]:
             #   lines.append("section is inside footer section")
              #  footer_detected = True
            # check if its an industry section: if it has 4 h4 tags and 4 a href tags
            elif len(section.find_all("h4")) == 4 and len(section.find_all("a")) == 4:
                lines.append("Industry section found: "+section.find("h4").text.strip())
                is_detected_after_footer(footer_detected)
            # check if its a group header section: contains only 1 h2
            elif len(section.find_all("h2")) == 1:
                if len(section.findChildren("section")) == 0: #should not have any child sections
                    h2 = section.find("h2").text.strip() 
                    if h2 == "Consistently Rated Best-in-Class":
                        lines.append("found section: Consistently Rated Best-in-Class") 
                        is_detected_after_footer(footer_detected)
                    elif h2 == "Schedule Your Demo Today!":
                        lines.append("schedule_demo_section");
                        is_detected_after_footer(footer_detected)
                    elif len(h2) > 4:
                        # has description?
                        descriptions = section.find_all("div", class_="elementor-widget-container")
                        description = find_longest_text(descriptions)
                        if len(description) > 50:
                            # group_header section detected
                            lines.append("group_header {")
                            lines.append("\theading \"" + h2 + "\"")
                            lines.append("\tsub_heading \"" + description + "\"")
                            lines.append("}")
                        else:
                            # group_header section only heading detected
                            lines.append("group_header {")
                            lines.append("\theading \"" + h2 + "\"")
                            lines.append("\tsub_heading \"\"")
                            lines.append("}")
                        is_detected_after_footer(footer_detected)
            # elif len(section.find_all("h2")) > 1:
            #     errors.append("Section with more than 1 h2:"+section.find("h2").text.strip())
            #     is_detected_after_footer(footer_detected)
            # elif len(section.find_all("h3")) > 1:
            #     errors.append("Section with more than 1 h3:"+section.find("h3").text.strip())
            #     is_detected_after_footer(footer_detected)
            # elif len(section.find_all("h4")) > 1:
            #     errors.append("Section with more than 1 h4:"+section.find("h4").text.strip())
            #     is_detected_after_footer(footer_detected)
            else:
                html = ' '.join([str(x) for x in section.find_all(text=True)]).strip()
                verbose.append("Ignored section HTML: "+ html)
                if len(html) > 0:
                    html = re.sub(r'[\s\n]+', ' ', html).strip()
                    errors.append("Error: Section containing this HTML was ignored: "+html)
                is_detected_after_footer(footer_detected)
    if idx == 40:
        break
with open("/Users/arohikulkarni/Work/Website Project/SourceFiles/pages/"+page_name[0]+".page", "w") as file:
    file.write(str("\n".join(lines).encode('utf8')).replace("b'", "").replace("\\r", "\r").replace("\\n", "\n").replace("\\t", "\t"))
with open("SiteExtraction/errors.txt", "w") as file:
    file.write(str("\n".join(errors).encode('utf8')).replace("b'", "").replace("\\r", "\r").replace("\\n", "\n").replace("\\t", "\t"))
with open("SiteExtraction/verbose.txt", "w") as file:
    file.write(str("\n".join(verbose).encode('utf8')).replace("b'", "").replace("\\r", "\r").replace("\\n", "\n").replace("\\t", "\t"))