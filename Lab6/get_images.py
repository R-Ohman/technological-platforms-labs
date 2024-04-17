import requests
import sys


def download_images(count):
    url = 'https://picsum.photos/4000/3000'
    for i in range(count):
        response = requests.get(url)
        with open(f'images/image_{i}.jpg', 'wb') as file:
            file.write(response.content)
        print(f'{i} of {count} images downloaded')


if __name__ == '__main__':
    if len(sys.argv) < 2:
        print('Usage: python get_images.py <count>')
        sys.exit(1)
    download_images(int(sys.argv[1]))
    