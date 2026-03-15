import random
import requests
import urllib3
from datetime import datetime, timedelta

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

API_URL = "https://localhost:7108/api/Lecturers"

TOKENS = {
    "admin1": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiTGV2ZWwiOiIxIiwiZXhwIjoxNzczNjI4NjUxLCJpc3MiOiJLaG9hQ05UVF9BUEkiLCJhdWQiOiJLaG9hQ05UVF9Vc2VyIn0.C2PHv8QMnZR6gyOhQGSfzf36Howw6IIgS1QQI306UJI",

    "admin2": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiTGV2ZWwiOiIyIiwiZXhwIjoxNzczNjI4NjYzLCJpc3MiOiJLaG9hQ05UVF9BUEkiLCJhdWQiOiJLaG9hQ05UVF9Vc2VyIn0.GvvhSOm04Z8v0odN9HpIDqlwOoa6g3m0n2JvEg3Rpko",
    
    "admin3": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiTGV2ZWwiOiIzIiwiZXhwIjoxNzczNjI4NjczLCJpc3MiOiJLaG9hQ05UVF9BUEkiLCJhdWQiOiJLaG9hQ05UVF9Vc2VyIn0.6S3E61dmKm9Bj5vAb2VP1hnt1EFlF7yIkfB6sputdg4"
}

SUBJECT_CODES = [
    "CSE105",
    "CSE111",
    "CSE112",
    "CSE106",
    "CSE115",
    "CSE124",
    "CSE126",
    "CSE123",
]

DEGREES = [
        "Bachelor",
        "Master",
        "Doctor", 
        "AssociateProfessor", 
        "Professor"
]

POSITIONS = [
    "Trưởng Bộ Môn",
    "Trưởng Khoa",
    "Giảng Viên"
]

FIRST_NAMES = [
    "An","Bình","Chi","Dũng","Hà","Hùng","Khánh","Lan",
    "Linh","Minh","Nam","Ngọc","Phong","Quang","Sơn",
    "Trang","Tuấn","Tú","Việt","Yến"
]

LAST_NAMES = [
    "Nguyễn","Trần","Lê","Phạm","Hoàng","Huỳnh",
    "Phan","Vũ","Đặng","Bùi","Đỗ"
]

import unicodedata

def remove_accents(text):
    return unicodedata.normalize('NFD', text)\
        .encode('ascii', 'ignore')\
        .decode('utf-8')

def random_name():
    return f"{random.choice(LAST_NAMES)} {random.choice(FIRST_NAMES)}"


def random_birthdate():
    start = datetime(1965,1,1)
    end = datetime(1995,12,31)

    delta = end - start
    days = random.randint(0, delta.days)

    return (start + timedelta(days=days)).isoformat() + "Z"


def random_phone():
    return "09" + "".join([str(random.randint(0,9)) for _ in range(8)])


def random_email(name):
    base = remove_accents(name).lower().replace(" ", "")
    num = random.randint(1,999)
    return f"{base}{num}@tlu.edu.vn"


def create_lecturer():

    name = random_name()

    payload = {
        "fullName": name,
        "imageUrl": f"https://i.pravatar.cc/300?u={random.randint(1,1000)}",
        "degree": random.choice(DEGREES),
        "position": random.choice(POSITIONS),
        "birthdate": random_birthdate(),
        "email": random_email(name),
        "phoneNumber": random_phone(),
        "subjectCodes": random.sample(SUBJECT_CODES, random.randint(1, len(SUBJECT_CODES)))
    }

    role = random.choice(list(TOKENS.keys()))
    token = TOKENS[role]

    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }

    try:
        res = requests.post(API_URL, json=payload, headers=headers, verify=False)
        print(f"{role} | {name} | {res.status_code}")

        if res.status_code != 200:
            print(payload)
            print(res.text)

    except Exception as e:
        print("Error:", e)


def main():

    for _ in range(25):
        create_lecturer()


if __name__ == "__main__":
    main()