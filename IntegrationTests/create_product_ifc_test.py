import requests
from urllib3.exceptions import InsecureRequestWarning

requests.packages.urllib3.disable_warnings(category=InsecureRequestWarning)

base_url = 'http://localhost:5000/api/create-ifc/product/'
product = 'test-product'
url = base_url + product
headers = {'Content-type': 'application/json'}

product_ifc_request = {
    "project": {
        "name": "test project",
        "description": "project for integration testing"
    },
    "owner": {
        "person": {
            "givenName": "Cedric",
            "familyName": "Mélange",
            "identifier": "cmela"
        },
        "organization": {
            "name": "test organization",
            "description": "organization for integration testing",
            "identifier": "org"
        },
        "application": {
            "name": "test application",
            "version": "1.0",
            "identifier": "testapp",
            "organization": {
                "name": "application organization",
                "description": "organization that produced the app",
                "identifier": "app org"
            }
        }
    },
    "product": {
        "type": "PROXY",
        "name": "test product",
        "description": "product for integration testing",
        "representations": [
            { "representationItems": [
                    {
                        "constructionString": "POLYGON_SHAPE([[[0,0],[0,1],[1,1],[1,0]]]).EXTRUDE(1)"
                    }
                ]
            }
        ]
    },
}

r = requests.post(url, json=product_ifc_request, headers=headers, verify=False)
print(str(r.status_code) + ' - ' r.text)
