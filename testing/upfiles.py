import os
import re
import random
import dotenv
dotenv.load_dotenv()
import requests
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

ROOT_FOLDER = r"E:\PDF\Programming"

API_URL = "https://localhost:7108/api/Files"

TOKENS = {
    "admin1": os.getenv("ADMIN1"),
    "admin2": os.getenv("ADMIN2"),
    "admin3": os.getenv("ADMIN3")
}

SUBJECT_CODES = [
    "CSE105",
    "CSE111",
    "CSE112",
    "CSE106",
    "CSE115",
    ""
]

PERMISSIONS = [
    "Hidden",
    "PublicRead",
    "PublicDownload",
    "StudentRead",
    "StudentDownload"
]

FILE_TYPES = [
    "LectureNotes",
    "Test",
    "Form",
    "Other"
]


def generate_title(filename):
    name = filename.rsplit(".", 1)[0]

    name = name.replace("_", " ")
    name = name.replace("-", " ")

    name = re.sub(r"\d+", "", name)
    name = re.sub(r"\s+", " ", name)

    name = name.strip()

    if len(name) > 60:
        name = name[:60]

    return name.title()


def collect_files(root):
    all_files = []

    for path, dirs, files in os.walk(root):
        for f in files:
            full = os.path.join(path, f)
            all_files.append(full)

    return all_files


def upload_file(filepath):

    filename = os.path.basename(filepath)

    title = generate_title(filename)

    subject = random.choice(SUBJECT_CODES)
    permission = random.choice(PERMISSIONS)
    filetype = random.choice(FILE_TYPES)

    role = random.choice(list(TOKENS.keys()))
    token = TOKENS[role]

    headers = {
        "Authorization": f"Bearer {token}"
    }

    files = {
        "File": (filename, open(filepath, "rb"))
    }

    data = {
        "Title": title,
        "SubjectCode": subject,
        "Permission": permission,
        "FileType": filetype
    }

    try:
        res = requests.post(API_URL, headers=headers, data=data, files=files, verify=False)

        print(f"{role} | {filename} | {title} | {res.status_code}")

    except Exception as e:
        print("Error:", filepath, e)


def main():

    files = collect_files(ROOT_FOLDER)

    random.shuffle(files)

    for f in files:
        upload_file(f)


if __name__ == "__main__":
    main()
