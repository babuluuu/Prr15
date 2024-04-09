from bs4 import BeautifulSoup
from locust import HttpUser, between, task
import random
import string

#Класс представляет тип имитируемого пользователя для теста Locust.
class TestLocust(HttpUser):
    #Случайное время ожидания от 1 до 2 секунд
    wait_time = between(1, 2)
    #Метод, который выполняет вход в систему  
    def on_start(self):
        self.login_users()
        self.csrf_token_create = self.fetch_csrf_token("/Admin/CreateQualification")
        self.edit_qualification_id = 1
        self.csrf_token_edit = self.fetch_csrf_token(f"/Admin/EditQualification/{self.edit_qualification_id}")
    
    def parse_csrf_token(self, html):
        soup = BeautifulSoup(html, 'html.parser')
        token_element = soup.find('input', {'name': '__RequestVerificationToken'})
        return token_element.get('value') if token_element else None

    def fetch_csrf_token(self, path):
        response = self.client.get(path)
        soup = BeautifulSoup(response.text, 'html.parser')
        token_element = soup.find('input', {'name': '__RequestVerificationToken'})
        return token_element.get('value') if token_element else None

    @task(10)
    def getGrades(self):
        self.client.get("/Grades/SelectGroupAndSubject")
    #Метод на правильность авторизации
    @task
    def login_users(self):
        self.client.post('/Home/Login', {'email': 'mamapapa@mpt.ru', 'password': '20042005'})
    #Метод на удаление пользователя
    @task
    def create_qualification(self):
        qualification_name = 'user-' + ''.join(random.choices(string.ascii_letters + string.digits, k=12))
        self.client.post("/Home/AdminView", {
            "Name": qualification_name,
            "__RequestVerificationToken": self.csrf_token_create
        }, headers={'Referer': '/Home/AdminView'})

    @task
    def edit_qualification(self):
        edit_qualification_id = random.randint(10, 30)
        response = self.client.get(f"/Admin/EditQualification/{edit_qualification_id}")
        soup = BeautifulSoup(response.text, 'html.parser')
        token_element = soup.find('input', {'name': '__RequestVerificationToken'})
        csrf_token_edit = token_element.get('value') if token_element else None

        if csrf_token_edit:
            new_name = 'Edited-' + ''.join(random.choices(string.ascii_letters + string.digits, k=5))
            self.client.post(f"/Admin/EditQualification/{edit_qualification_id}", {
                "QualificationId": edit_qualification_id,
                "Name": new_name,
                "__RequestVerificationToken": csrf_token_edit
            }, headers={'Referer': f'/Admin/EditQualification/{edit_qualification_id}'})

    @task
    def delete_qualification(self):
        qualification_id = random.randint(12, 45)
        with self.client.get(f"/Admin/DeleteQualification/{qualification_id}", catch_response=True) as get_response:
            if get_response.status_code == 200:
                token = self.parse_csrf_token(get_response.text)
                self.client.post(f"/Admin/DeleteQualification/{qualification_id}", {
                    '__RequestVerificationToken': token
                }, headers={'Referer': f"/Admin/DeleteQualification/{qualification_id}"})